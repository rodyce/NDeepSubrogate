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
