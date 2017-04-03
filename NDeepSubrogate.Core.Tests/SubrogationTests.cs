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
using NDeepSubrogate.Core.Attributes;
using NDeepSubrogate.Core.Tests.SampleClasses;
using NDeepSubrogate.Core.Tests.SampleInterfaces;
using NDeepSubrogate.Core.Tests.SampleInterfaces.Impl;
using NSubstitute;
using NUnit.Framework;

namespace NDeepSubrogate.Core.Tests
{
    [TestFixture]
    public class SubrogationTests
    {

        [Test]
        public void DetermineTypesToSubrogateTest()
        {
            //Since only the _calculator field, of type ICalculator, has the Subrogate attribute then
            //only the type ICalculator must be subrogated.

            var dummy = new DummyOneSurrogate();

            Func<Type, object> dummyFunc = type => null;
            var surrogateScope = new DeepSurrogateScope(dummy, dummyFunc, dummyFunc);

            var expectedSet = new HashSet<Type>() { typeof(ICalculator) };
            Assert.AreEqual(expectedSet, surrogateScope.TypesToSubrogateSet);
        }

        [Test]
        public void SubrogateTest()
        {
            var dummy = new DummyTwoSurrogates();
            var calculatorSubstitute = Substitute.For<ICalculator>();
            var vehicleSubstitute = Substitute.For<Vehicle>();

            IDictionary<Type, object> originalObjects = new Dictionary<Type, object>
            {
                {typeof(ICalculator), dummy.Calculator},
                {typeof(Vehicle), dummy.Vehicle}
            };
            Func<Type, object> originalObjectsFunc = type => originalObjects[type];

            IDictionary<Type, object> replacementObjects = new Dictionary<Type, object>
            {
                {typeof(ICalculator), calculatorSubstitute},
                {typeof(Vehicle), vehicleSubstitute}
            };
            Func<Type, object> replacementObjectsFunc = type => replacementObjects[type];

            var surrogateScope = new DeepSurrogateScope(dummy, originalObjectsFunc, replacementObjectsFunc);

            surrogateScope.DeepSubrogate();
            Assert.AreSame(replacementObjects[typeof(ICalculator)], dummy.Calculator);
            Assert.AreSame(replacementObjects[typeof(Vehicle)], dummy.Vehicle);

            surrogateScope.DeepRestore();
            Assert.AreSame(originalObjects[typeof(ICalculator)], dummy.Calculator);
            Assert.AreSame(originalObjects[typeof(Vehicle)], dummy.Vehicle);
        }

        [DeepSubrogate]
        private class DummyOneSurrogate
        {
            [Subrogate]
            private readonly ICalculator _calculator = new Calculator();
            private readonly Vehicle _vehicle = new Vehicle();

            public ICalculator Calculator => _calculator;
            public Vehicle Vehicle => _vehicle;
        }

        [DeepSubrogate]
        private class DummyTwoSurrogates
        {
            [Subrogate]
            private readonly ICalculator _calculator = new Calculator();
            [Subrogate]
            private readonly Vehicle _vehicle = new Vehicle();

            public ICalculator Calculator => _calculator;
            public Vehicle Vehicle => _vehicle;
        }
    }
}
