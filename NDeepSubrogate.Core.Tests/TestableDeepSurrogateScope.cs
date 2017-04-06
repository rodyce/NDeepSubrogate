using System;

namespace NDeepSubrogate.Core.Tests
{
    internal class TestableDeepSurrogateScope : DeepSurrogateScope
    {
        private readonly Func<Type, object> _surrogateObjectsFunc;
        private readonly Func<Type, object> _originalObjectsFunc;

        public TestableDeepSurrogateScope(object initialObject,
            Func<Type, object> surrogateObjectsFunc,
            Func<Type, object> originalObjectsFunc) : base(initialObject)
        {
            _surrogateObjectsFunc = surrogateObjectsFunc;
            _originalObjectsFunc = originalObjectsFunc;
        }

        protected override object GetObjectFromType(Type type)
        {
            return _originalObjectsFunc(type);
        }

        protected override object GetSurrogateFromType(Type type)
        {
            return _surrogateObjectsFunc(type);
        }
    }
}
