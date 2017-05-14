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
using NUnit.Framework;
using FakeItEasy;
using NDeepSubrogate.Core.Tests.TestUtils;

namespace NDeepSubrogate.Core.Tests
{
    [TestFixture]
    internal class SubrogationTests
    {
        private ICalculator _calculatorSurrogate;
        private Vehicle _vehicleSurrogate;
        private IDictionary<Type, object> _typeToSurrogateDictionary;
        private IDictionary<Type, object> _typeToObjectDictionary;

        [SetUp]
        protected void Init()
        {
            _calculatorSurrogate = A.Fake<ICalculator>();
            _vehicleSurrogate = A.Fake<Vehicle>();

            _typeToSurrogateDictionary = new Dictionary<Type, object>
            {
                {typeof(ICalculator), _calculatorSurrogate},
                {typeof(Vehicle), _vehicleSurrogate}
            };

            _typeToObjectDictionary = new Dictionary<Type, object>();
        }

        private object GetSurrogateFromType(Type type)
        {
            return _typeToSurrogateDictionary[type];
        }

        private object GetObjectFromType(Type type)
        {
            return _typeToObjectDictionary[type];
        }


        [Test]
        public void Subrogation_SimpleSubrogateAndRestoreTest()
        {
            // Instantiate a dummy object referencing other objects to be subrogated
            var dummy = new DummyTwoSurrogates();

            // Instantiate a new subrogation scope with the previous dummy object as the
            // scope's initial object
            var surrogateScope = new TestableDeepSurrogateScope(dummy, GetSurrogateFromType,
                GetObjectFromType);

            // Add the original referenced objects by dummy to be restored later
            _typeToObjectDictionary.Add(typeof(ICalculator), dummy.Calculator);
            _typeToObjectDictionary.Add(typeof(Vehicle), dummy.Vehicle);

            // Method under test
            surrogateScope.DeepSubrogate();

            // Verify that the Calculator and Vehicule objects were subrogated
            Assert.AreSame(GetSurrogateFromType(typeof(ICalculator)), dummy.Calculator);
            Assert.AreSame(GetSurrogateFromType(typeof(Vehicle)), dummy.Vehicle);
            Assert.AreNotSame(GetObjectFromType(typeof(ICalculator)), dummy.Calculator);
            Assert.AreNotSame(GetObjectFromType(typeof(Vehicle)), dummy.Vehicle);

            // Method under test
            surrogateScope.DeepRestore();

            // Verify that the original objects were all restored
            Assert.AreNotSame(GetSurrogateFromType(typeof(ICalculator)), dummy.Calculator);
            Assert.AreNotSame(GetSurrogateFromType(typeof(Vehicle)), dummy.Vehicle);
            Assert.AreSame(GetObjectFromType(typeof(ICalculator)), dummy.Calculator);
            Assert.AreSame(GetObjectFromType(typeof(Vehicle)), dummy.Vehicle);
        }

        [Test]
        public void Subrogation_DeepReferencesSubrogateAndRestoreTest()
        {
            // Instantiate a dummy object referencing other objects to be potentially
            // subrogated
            var dummy = new DummyOneSurrogate();

            // Instantiate a new subrogation scope with the previous dummy object as the
            // scope's initial object
            var surrogateScope = new TestableDeepSurrogateScope(dummy, GetSurrogateFromType,
                GetObjectFromType);

            // Add the original referenced objects by dummy to be restored later
            _typeToObjectDictionary.Add(typeof(ICalculator), dummy.Calculator);
            _typeToObjectDictionary.Add(typeof(Vehicle), dummy.Vehicle);

            // Method under test
            surrogateScope.Execute(() =>
            {
                // Verify that the Calculator objects was subrogated
                Assert.AreSame(GetSurrogateFromType(typeof(ICalculator)), dummy.Calculator);
                Assert.AreNotSame(GetObjectFromType(typeof(ICalculator)), dummy.Calculator);

                // Verify that the Vehicle object was Not subrogated (it does not contain the SubrogateAttribute)
                Assert.AreSame(GetObjectFromType(typeof(Vehicle)), dummy.Vehicle);

                // Verify that the Calculator object referenced by the Vehicle object was subrogated
                Assert.AreSame(GetSurrogateFromType(typeof(ICalculator)), dummy.Vehicle.Calculator);
                Assert.AreNotSame(GetObjectFromType(typeof(ICalculator)), dummy.Vehicle.Calculator);
            });

            // Verify that the original objects were all restored
            Assert.AreSame(GetObjectFromType(typeof(ICalculator)), dummy.Calculator);
            Assert.AreNotSame(GetSurrogateFromType(typeof(ICalculator)), dummy.Calculator);

            // Verify that the Calculator object referenced by the Vehicle object was restored
            Assert.AreSame(GetObjectFromType(typeof(ICalculator)), dummy.Vehicle.Calculator);
            Assert.AreNotSame(GetSurrogateFromType(typeof(ICalculator)), dummy.Vehicle.Calculator);
        }

        [Test]
        public void Subrogation_NoSurrogatesTest()
        {
            // Instantiate a dummy object referencing other objects to be potentially
            // subrogated
            var dummy = new DummyNoSurrogate();

            // Instantiate a new subrogation scope with the previous dummy object as the
            // scope's initial object
            var surrogateScope = new TestableDeepSurrogateScope(dummy, GetSurrogateFromType,
                GetObjectFromType);

            // Add the original referenced objects by dummy to be restored later
            _typeToObjectDictionary.Add(typeof(ICalculator), dummy.Calculator);
            _typeToObjectDictionary.Add(typeof(Vehicle), dummy.Vehicle);

            // Method under test
            surrogateScope.Execute(() =>
            {
                // Verify that neither the Calculator nor the Vehicule object were subrogated
                // as the fields that reference them do not have the SubrogateAttribute.
                Assert.AreSame(GetObjectFromType(typeof(ICalculator)), dummy.Calculator);
                Assert.AreNotSame(GetSurrogateFromType(typeof(ICalculator)), dummy.Calculator);
                Assert.AreSame(GetObjectFromType(typeof(Vehicle)), dummy.Vehicle);
                Assert.AreNotSame(GetSurrogateFromType(typeof(Vehicle)), dummy.Vehicle);
            });
        }
    }
}
