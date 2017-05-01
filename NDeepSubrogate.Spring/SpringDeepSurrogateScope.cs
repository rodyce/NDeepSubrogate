#region License

/*
 * Copyright 2017 Rodimiro Cerrato Espinal.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NDeepSubrogate.Core;
using NDeepSubrogate.Core.Attributes;
using Spring.Aop.Framework;
using Spring.Context.Support;
using Spring.Objects.Factory.Attributes;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Support;

namespace NDeepSubrogate.Spring
{
    public class SpringDeepSurrogateScope : DeepSurrogateScope
    {
        private readonly AbstractApplicationContext _applicationContext;
        private readonly IDictionary<string, IObjectDefinition> _originalObjectDefinitionsDictionary;

        public SpringDeepSurrogateScope(object initialObject,
            AbstractApplicationContext applicationContext) : base(initialObject)
        {
            _applicationContext = applicationContext;
            _originalObjectDefinitionsDictionary = new Dictionary<string, IObjectDefinition>();
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
                .AddPropertyValue("TargetObjectType", type)
                .ObjectDefinition;

            var nameList = objectNameList as string[] ?? objectNameList.ToArray();
            var objectDefinitionName = nameList.Count() == 1 ? nameList.First() : type.FullName;
            if (_applicationContext.ContainsObjectDefinition(objectDefinitionName))
            {
                // Save the original object definition so it can be restored afterwards.
                _originalObjectDefinitionsDictionary[objectDefinitionName] =
                    _applicationContext.GetObjectDefinition(objectDefinitionName);
            }
            else
            {
                //This version of Spring.NET does not support removing object definition from the ApplicationContext.
                //Therefore, do not support this for now.
                //Once supported, add the surrogate ObjectDefinition to be removed later.
                throw new NotSupportedException("Cannot register a surrogate ObjectDefinition since it cannot be " +
                                                "removed afterwards");
            }
            _applicationContext.ReplaceObjectDefinition(objectDefinitionName, objectDefinition);
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
            foreach (var entry in _originalObjectDefinitionsDictionary)
            {
                _applicationContext.ReplaceObjectDefinition(entry.Key, entry.Value);
            }
        }

        private IEnumerable<string> GetAppContextObjectNameListFromType(Type type)
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
