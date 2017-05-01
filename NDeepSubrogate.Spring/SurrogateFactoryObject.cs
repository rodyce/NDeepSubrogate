using System;
using FakeItEasy.Sdk;
using Spring.Objects.Factory.Config;

namespace NDeepSubrogate.Spring
{
    internal class SurrogateFactoryObject : AbstractFactoryObject
    {
        private Type _objectType;

        public override Type ObjectType => _objectType;

        public Type TargetObjectType
        {
            set { _objectType = value; }
        }

        public object ObjectToFake { get; set; } = null;

        protected override object CreateInstance()
        {
            return ObjectToFake == null ?
                Create.Fake(ObjectType) :
                Create.Fake(ObjectType, x => x.Wrapping(ObjectToFake));
        }
    }
}
