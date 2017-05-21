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
using NDeepSubrogate.Core.Tests.SampleClasses;
using NDeepSubrogate.Core.Tests.SampleInterfaces;
using NDeepSubrogate.Core.Tests.TestUtils;
using NUnit.Framework;

namespace NDeepSubrogate.Core.Tests
{
    [TestFixture]
    internal class SurrogateTypesDeterminationTests
    {
        [Test]
        public void TypesToSubrogate_SomeFieldsWithSubrogateAttributeTest()
        {
            var dummy = new DummyOneSurrogate();

            var surrogateScope = new TestableDeepSubrogationScope(dummy, type => null, type => null);

            // Since only the _calculator field (ICalculator type), has the SubrogateAttribute then
            // only the ICalculator type fields must be subrogated.
            var expectedSet = new HashSet<Type>() { typeof(ICalculator) };

            // Property under test
            Assert.AreEqual(expectedSet, surrogateScope.TypesToSubrogateSet);
        }

        [Test]
        public void TypesToSubrogate_AllFieldsWithSubrogateAttributeTest()
        {
            var dummy = new DummyTwoSurrogates();

            var surrogateScope = new TestableDeepSubrogationScope(dummy, type => null, type => null);

            // Since both the _calculator field (ICalculator), and the _vehicle field (Vehicle)
            // have the SubrogateAttribute then both fields must be subrogated.
            var expectedSet = new HashSet<Type>() { typeof(ICalculator), typeof(Vehicle) };

            // Property under test
            Assert.AreEqual(expectedSet, surrogateScope.TypesToSubrogateSet);
        }

        [Test]
        public void TypesToSubrogate_NoFieldWithSubrogateAttributeTest()
        {
            var dummy = new DummyNoSurrogate();

            var surrogateScope = new TestableDeepSubrogationScope(dummy, type => null, type => null);

            // Since none of the fields have the SubrogateAttribute, then nothing must be subrogated.
            var expectedSet = new HashSet<Type>();

            // Property under test
            Assert.AreEqual(expectedSet, surrogateScope.TypesToSubrogateSet);
        }

        [Test]
        public void SubrogationDisabledTest()
        {
            var dummy = new DummySubrogationDisabled();

            var surrogateScope = new TestableDeepSubrogationScope(dummy, type => null, type => null);

            // Since the DummySubrogationDisabled class has the [DeepSubrogate] attribute with the
            // Enabled flag set to false, then the scope's TypesToSubrogateSet must be empty.
            var expectedSet = new HashSet<Type>();

            // Property under test
            Assert.AreEqual(expectedSet, surrogateScope.TypesToSubrogateSet);
        }
    }
}
