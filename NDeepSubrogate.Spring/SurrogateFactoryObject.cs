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

        public SurrogateType SurrogateType { get; set; } = SurrogateType.Mock;

        public object ObjectToFake { get; set; } = null;

        protected override object CreateInstance()
        {
            object surrogateObject;
            switch (SurrogateType)
            {
                case SurrogateType.Mock:
                    if (ObjectToFake != null)
                    {
                        throw new InvalidOperationException("A Mock surrogate does not need an underlying object.");
                    }
                    surrogateObject = Create.Fake(ObjectType);
                    break;
                case SurrogateType.Spy:
                    if (ObjectToFake == null)
                    {
                        throw new InvalidOperationException("A Spy surrogate must have an underlying object to spy.");
                    }
                    surrogateObject = Create.Fake(ObjectType, x => x.Wrapping(ObjectToFake));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Need to specify the type of surrogate (Mock or Spy)");
            }
            return surrogateObject;
        }
    }

    internal enum SurrogateType
    {
        Mock,
        Spy
    }
}
