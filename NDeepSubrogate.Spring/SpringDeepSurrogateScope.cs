using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NDeepSubrogate.Core;
using NDeepSubrogate.Core.Attributes;
using Spring.Aop.Framework;
using Spring.Context.Support;
using Spring.Core.IO;
using Spring.Objects.Factory.Attributes;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Support;

namespace NDeepSubrogate.Spring
{
    public class SpringDeepSurrogateScope : DeepSurrogateScope
    {
        private readonly AbstractApplicationContext _applicationContext;
        private readonly ISet<string> _objectDefsToRemoveSet;
        private readonly IDictionary<string, IObjectDefinition> _oldObjectDefinitionsDictionary;

        public SpringDeepSurrogateScope(object initialObject,
            AbstractApplicationContext applicationContext) : base(initialObject)
        {
            _applicationContext = applicationContext;
            _objectDefsToRemoveSet = new HashSet<string>();
            _oldObjectDefinitionsDictionary = new Dictionary<string, IObjectDefinition>();
        }

        public override void DeepSubrogate()
        {
            if (TypesToSubrogateSet.Count == 0)
            {
                //Bail out early if there is nothing to subrogate
                return;
            }
            foreach (var type in TypesToSubrogateSet)
            {
                MockApplicationContextForType(type);
            }
            base.DeepSubrogate();
        }

        public override void DeepRestore()
        {
            RestoreAppContext();
            base.DeepRestore();
        }

        private void MockApplicationContextForType(Type type)
        {
            if (!TypesToSubrogateSet.Contains(type))
            {
                return;
            }

            var objectNameList = GetAppContextObjectNameListFromType(type);

            var objectDefinition = ObjectDefinitionBuilder.GenericObjectDefinition(
                typeof(SurrogateFactoryObject))
                .AddPropertyValue("ObjectType", type)
                .ObjectDefinition;
            var objectDefinitionName = objectNameList.Count == 1 ? objectNameList.First() : type.FullName;
            if (_applicationContext.ContainsObjectDefinition(objectDefinitionName))
            {
                //TODO: Save original object definition
                _oldObjectDefinitionsDictionary[objectDefinitionName] =
                    _applicationContext.GetObjectDefinition(objectDefinitionName);
                _applicationContext.CreateObject(objectDefinitionName, null, null);
            }
            else
            {
                //Add the surrogate ObjectDefinition to be removed later
                _objectDefsToRemoveSet.Add(objectDefinitionName);

                //This version of Spring.NET does not support removing object definition from the ApplicationContext.
                //Therefore, do not support this for now.
                throw new NotSupportedException("Cannot register a surrogate ObjectDefinition since it cannot be " +
                                                "removed");
            }
            _applicationContext.RegisterObjectDefinition(objectDefinitionName, objectDefinition);
            _applicationContext.CreateObject(objectDefinitionName, null, null);
            _applicationContext.GetObject(objectDefinitionName);
        }

        protected override object GetObjectFromType(Type type)
        {
            var objectNameList = GetAppContextObjectNameListFromType(type);
            return _applicationContext.GetObject(objectNameList.First());
        }

        protected override object GetSurrogateFromType(Type type)
        {
            return GetObjectFromType(type);
        }

        protected override object GetObjectFromField(FieldInfo fieldInfo, object o)
        {
            var target = fieldInfo.GetValue(o);
            return AopUtils.IsAopProxy(target) ?
                ((IAdvised) target).TargetSource.GetTarget() :
                target;
        }

        protected override bool IsFieldToBeExplored(FieldInfo field)
        {
            //TODO consider fields that contains existent Object Definition in the application
            //context. i.e.:
            //_applicationContext.ContainsObject(GetComponentNameFromField(field));
            //Examine fields with the [Autowired] attribute and without the [Subrogate] attribute
            //TODO Consider partial mocks
            return field.GetCustomAttribute<AutowiredAttribute>() != null &&
                   field.GetCustomAttribute<SubrogateAttribute>() == null;
        }

        private static string GetComponentNameFromField(FieldInfo field)
        {
            var qualifierAttribute = field.GetCustomAttribute<QualifierAttribute>();
            return qualifierAttribute != null ? qualifierAttribute.Value : field.FieldType.FullName;
        }

        private void RestoreAppContext()
        {
            foreach (var objectDefinitionName in _objectDefsToRemoveSet)
            {
                _applicationContext.RegisterObjectDefinition(objectDefinitionName,
                    _oldObjectDefinitionsDictionary[objectDefinitionName]);
            }
        }

        private IList<string> GetAppContextObjectNameListFromType(Type type)
        {
            var objectNameList = _applicationContext.GetObjectNamesForType(type);
            if (objectNameList.Count > 1)
            {
                throw new NotSupportedException("The case where there is more than one object for a single type is " +
                                                "not yet supported");
            }
            return objectNameList;
        }
    }
}
