using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NDeepSubrogate.Core.Attributes;

namespace NDeepSubrogate.Core
{
    public class DeepSurrogateScope
    {
        private readonly object _initialObject;
        private readonly IDictionary<Type, ISet<FieldInfo>> _fieldsToRestoreDictionary;
        private readonly Func<Type, object> _getObjectFromTypeFunc;
        private readonly Func<Type, object> _getSurrogateFromTypeFunc;
        private readonly ISet<object> _processedObjects;

        public DeepSurrogateScope(object initialObject,
            Func<Type, object> getObjectFromTypeFunc, Func<Type, object> getSurrogateFromTypeFunc)
        {
            _initialObject = initialObject;
            _fieldsToRestoreDictionary = new Dictionary<Type, ISet<FieldInfo>>();
            _processedObjects = new HashSet<object>();
            _getObjectFromTypeFunc = getObjectFromTypeFunc;
            _getSurrogateFromTypeFunc = getSurrogateFromTypeFunc;
            TypesToSubrogateSet = DetermineTypesToSubrogate();
        }

        public ISet<Type> TypesToSubrogateSet { get; }

        public void DeepSubrogate()
        {
            DeepSubrogateReferences(_initialObject);
        }

        private void DeepSubrogateReferences(object currentObject)
        {
            if (_processedObjects.Contains(currentObject))
            {
                return;
            }
            _processedObjects.Add(currentObject);
            var workingType = currentObject.GetType();
            var fieldSet = GetAllFieldsFromType(workingType);
            foreach (var field in fieldSet)
            {
                if (IsFieldToBeSubstituted(field))
                {
                    //Register the current field and its owner type to be restored
                    RegisterFieldToRestore(workingType, field);

                    var replacementObject = _getSurrogateFromTypeFunc(field.FieldType);

                    //TODO: Determine if replacement object needs to be explored for mocking

                    field.SetValue(currentObject, replacementObject);
                }
                else if (IsFieldToBeExplored(field))
                {
                    
                }
            }
        }

        public void DeepRestore()
        {
            ForEachFieldToRestore((type, fieldInfo) =>
            {
                var instance = type == _initialObject.GetType() ? _initialObject : _getObjectFromTypeFunc(type);

                if (instance != null)
                {
                    fieldInfo.SetValue(instance, _getObjectFromTypeFunc(fieldInfo.FieldType));
                }
            });
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

        protected bool IsFieldToBeSubstituted(FieldInfo field)
        {
            return TypesToSubrogateSet.Contains(field.FieldType);
        }

        protected bool IsFieldToBeExplored(FieldInfo field)
        {
            return !IsFieldToBeSubstituted(field);
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

        protected ISet<Type> DetermineTypesToSubrogate()
        {
            var typesToSubrogate = new HashSet<Type>();
            var workingType = _initialObject.GetType();
            var fieldSet = GetAllFieldsFromType(workingType);
            foreach (var field in fieldSet)
            {
                foreach (var customAttributeData in field.CustomAttributes)
                {
                    if (customAttributeData.AttributeType == typeof(SubrogateAttribute))
                    {
                        typesToSubrogate.Add(field.FieldType);
                    }
                }
            }

            return typesToSubrogate;
        }
    }
}
