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
using System.Reflection;
using NDeepSubrogate.Core.Attributes;

namespace NDeepSubrogate.Core
{
    public abstract class DeepSubrogationScope
    {
        private readonly object _initialObject;
        private readonly IDictionary<Type, ISet<FieldInfo>> _fieldsToRestoreDictionary;
        private readonly ISet<object> _processedObjects;

        protected DeepSubrogationScope(object initialObject)
        {
            _initialObject = initialObject;
            _fieldsToRestoreDictionary = new Dictionary<Type, ISet<FieldInfo>>();
            _processedObjects = new HashSet<object>();

            DetermineTypesToSubrogate();
        }

        public ISet<Type> TypesToSubrogateSet { get; private set; }

        public ISet<Type> TypesToSpySubrogateSet { get; private set; }

        public virtual void DeepSubrogate()
        {
            _fieldsToRestoreDictionary.Clear();
            _processedObjects.Clear();

            DeepSubrogateReferences(_initialObject);
        }

        public virtual void DeepRestore()
        {
            ForEachFieldToRestore((type, fieldInfo) =>
            {
                var instance = type == _initialObject.GetType() ? _initialObject : GetObjectFromType(type);

                if (instance != null)
                {
                    fieldInfo.SetValue(instance, GetObjectFromType(fieldInfo.FieldType));
                }
            });
            _fieldsToRestoreDictionary.Clear();
            _processedObjects.Clear();
        }

        public virtual void Execute(Action action)
        {
            try
            {
                // Deeply subrogate the scope
                DeepSubrogate();

                // Invoke the user supplied action within the subrogation scope.
                action();
            }
            finally
            {
                // Just rethrow any exception as a result of any exception in .
                // Restore the scope that was subrogated previously. Any thrown exception within
                // the user-supplied action invocation will just get rethrown. If an exception
                // was thrown as a consequence of invoking the DeepSubrogate() call, any partial
                // subrogation will be restored here.
                DeepRestore();
            }
        }

        private void DeepSubrogateReferences(object currentObject)
        {
            if (currentObject == null || _processedObjects.Contains(currentObject) ||
                !IsSubrogationEnabledForType(currentObject.GetType()))
            {
                return;
            }
            _processedObjects.Add(currentObject);
            var workingType = currentObject.GetType();
            var fieldSet = GetAllFieldsFromType(workingType);
            foreach (var field in fieldSet)
            {
                if (IsFieldToBeSubrogated(field))
                {
                    //Register the current field and its owner type to be restored
                    RegisterFieldToRestore(workingType, field);

                    var surrogateObject = GetSurrogateFromType(field.FieldType);

                    field.SetValue(currentObject, surrogateObject);
                }

                // Note: Spy surrogates must be explored. Therefore, this condition
                // here has to be evaluated even if the previous one happened to be
                // evaluated as true.
                if (IsFieldToBeExplored(field))
                {
                    var obj = GetObjectFromField(field, currentObject);
                    DeepSubrogateReferences(obj);
                }
            }
        }


        private static IEnumerable<FieldInfo> GetAllFieldsFromType(Type type)
        {
            var allFields = new HashSet<FieldInfo>();
            for (var t = type; t != null; t = t.BaseType)
            {
                foreach (var field in t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    allFields.Add(field);
                }
            }
            return allFields;
        }

        protected virtual bool IsFieldToBeSubrogated(FieldInfo field)
        {
            return TypesToSubrogateSet.Contains(field.FieldType);
        }

        protected virtual bool IsFieldToBeSpySubrogated(FieldInfo field)
        {
            return TypesToSpySubrogateSet.Contains(field.FieldType);
        }

        protected virtual bool IsFieldToBeExplored(FieldInfo field)
        {
            return !IsFieldToBeSubrogated(field);
        }

        private void RegisterFieldToRestore(Type type, FieldInfo field)
        {
            if (!_fieldsToRestoreDictionary.ContainsKey(type))
            {
                _fieldsToRestoreDictionary[type] = new HashSet<FieldInfo>();
            }
            _fieldsToRestoreDictionary[type].Add(field);
        }

        private void ForEachFieldToRestore(Action<Type, FieldInfo> restoreFunc)
        {
            foreach (var dictEntry in _fieldsToRestoreDictionary)
            {
                foreach (var field in dictEntry.Value)
                {
                    restoreFunc(dictEntry.Key, field);
                }
            }
        }

        private static bool IsSubrogationEnabledForType(Type someType)
        {
            var attr = someType.GetCustomAttribute<DeepSubrogateAttribute>();
            return attr == null || attr.Enabled;
        }

        private void DetermineTypesToSubrogate()
        {
            //The TypesToSubrogateSet and TypesToSpySubrogateSet properties may not be null.
            //If there is nothing to subrogate, then these are empty sets.
            TypesToSubrogateSet = new HashSet<Type>();
            TypesToSpySubrogateSet = new HashSet<Type>();

            var workingType = _initialObject.GetType();
            if (!IsSubrogationEnabledForType(workingType))
            {
                return;
            }

            var fieldSet = GetAllFieldsFromType(workingType);
            foreach (var field in fieldSet)
            {
                var subrogateAttr = field.GetCustomAttribute<SubrogateAttribute>();
                if (subrogateAttr == null)
                {
                    continue;
                }

                TypesToSubrogateSet.Add(field.FieldType);
                if (subrogateAttr.Spy)
                {
                    TypesToSpySubrogateSet.Add(field.FieldType);
                }
            }
        }

        protected abstract object GetObjectFromType(Type type);

        protected abstract object GetSurrogateFromType(Type type);

        protected virtual object GetObjectFromField(FieldInfo fieldInfo, object o)
        {
            return fieldInfo.GetValue(o);
        }
    }
}
