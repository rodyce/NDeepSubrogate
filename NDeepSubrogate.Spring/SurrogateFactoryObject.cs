using System;
using FakeItEasy.Sdk;
using Spring.Objects.Factory;

namespace NDeepSubrogate.Spring
{
    internal class SurrogateFactoryObject : IFactoryObject
    {
        public object GetObject()
        {
            return ObjectToFake == null ?
                Create.Fake(ObjectType) :
                Create.Fake(ObjectType, x => x.Wrapping(ObjectToFake));
        }

        public Type ObjectType { get; set; } = typeof(object);

        public object ObjectToFake { get; set; } = null;

        public bool IsSingleton { get; } = true;
    }
}
