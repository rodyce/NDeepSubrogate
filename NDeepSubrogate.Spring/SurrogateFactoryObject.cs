using System;
using NSubstitute;
using NSubstitute.Core;
using Spring.Objects.Factory;

namespace NDeepSubrogate.Spring
{
    internal class SurrogateFactoryObject : IFactoryObject
    {
        public object GetObject()
        {
            var objectTypeArray = new[] {ObjectType};

            return ObjectType.IsInterface
                ? Substitute.For(objectTypeArray, null)
                : SubstitutionContext.Current.SubstituteFactory.CreatePartial(objectTypeArray, null);
        }

        public Type ObjectType { get; set; } = typeof(object);

        public bool IsSingleton { get; } = true;
    }
}
