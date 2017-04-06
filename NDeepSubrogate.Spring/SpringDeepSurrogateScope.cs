using System;
using System.Collections.Generic;
using System.Reflection;
using NDeepSubrogate.Core;
using Spring.Aop.Framework;
using Spring.Context.Support;
using Spring.Objects.Factory.Attributes;
using Spring.Objects.Factory.Support;

namespace NDeepSubrogate.Spring
{
    public class SpringDeepSurrogateScope : DeepSurrogateScope
    {
        private readonly GenericApplicationContext _applicationContext;
        private ISet<string> _objectDefsToRemoveSet;

        private void MockApplicationContextForType(Type type)
        {
            if (!TypesToSubrogateSet.Contains(type))
            {
                
                return;
            }

            var objectDefinition = ObjectDefinitionBuilder.GenericObjectDefinition(
                typeof(SurrogateFactoryObject))
                .AddPropertyValue("ObjectType", type)
                .ObjectDefinition;
            var objectDefinitionName = type.FullName;
            if (_applicationContext.ContainsObjectDefinition(objectDefinitionName))
            {
                //TODO: Save original object definition
            }
            else
            {
                //TODO: Add the surrogate ObjectDefinition to be removed later
            }
            _applicationContext.RegisterObjectDefinition(objectDefinitionName, objectDefinition);
        }

        public SpringDeepSurrogateScope(object initialObject,
            GenericApplicationContext applicationContext) : base(initialObject)
        {
            _applicationContext = applicationContext;
        }

        protected override object GetObjectFromType(Type type)
        {
            throw new NotImplementedException();
        }

        protected override object GetSurrogateFromType(Type type)
        {
            throw new NotImplementedException();
        }

        protected override object GetObjectFromField(FieldInfo fieldInfo, object o)
        {
            var target = fieldInfo.GetValue(o);
            return AopUtils.IsAopProxy(target) ?
                ((IAdvised) target).TargetSource.GetTarget() :
                target;
        }

        private string GetComponentNameFromField(FieldInfo field)
        {
            var qualifierAttribute = field.GetCustomAttribute<QualifierAttribute>();
            return qualifierAttribute != null ? qualifierAttribute.Value : field.FieldType.Name;
        }
    }
}
