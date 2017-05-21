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


using FakeItEasy;
using NDeepSubrogate.Core.Attributes;
using NDeepSubrogate.Core.Tests.SampleInterfaces;
using NDeepSubrogate.Spring.Testing.NUnit;
using NDeepSubrogate.Spring.Tests.ServiceImpl;
using NUnit.Framework;
using Spring.Objects.Factory.Attributes;

namespace NDeepSubrogate.Spring.Tests
{
    public class SubrogationInDiSpringContextTests : SubrogationEnabledDependencyInjectionSpringContextTests
    {
        public SubrogationInDiSpringContextTests()
        {
            ConfigLocations = new[] { "context.xml" };
        }

        protected override string[] ConfigLocations { get; }

        [Subrogate]
        [Autowired]
        private readonly ICalculator _dummyCalculator = null;

        [Autowired]
        private readonly SpringVehicle _vehicle = null;

        [Test]
        public void ServicesSubrogatedInTestMethodTest()
        {
            Assert.IsNotNull(_dummyCalculator);
            Assert.IsNotNull(_vehicle);

            // Test
            A.CallTo(() => _dummyCalculator.Add(A<double>._, A<double>._))
                .MustNotHaveHappened();

            A.CallTo(() => _dummyCalculator.Add(A<double>._, A<double>._))
                .Returns(100.0);

            // Make sure vehicle's speed is 0.0 Km/h (stopped)
            _vehicle.Stop();
            Assert.AreEqual(0, _vehicle.SpeedInKph);

            // Accelerate the vehicle by 10 and then 40 Km/h.
            _vehicle.Accelerate(10);
            _vehicle.Accelerate(40);
            // Expected result here is 100.0, instead of 50.0. This is because every time we add
            // two doubles, the result is always 100.0, as configured before.
            Assert.AreEqual(100, _vehicle.SpeedInKph);

            A.CallTo(() => _dummyCalculator.Add(A<double>._, A<double>._))
                .MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Test]
        public void SurrogateServicesCleanedInTestMethodTest()
        {
            Assert.IsNotNull(_dummyCalculator);
            Assert.IsNotNull(_vehicle);

            A.CallTo(() => _dummyCalculator.Add(A<double>._, A<double>._))
                .MustNotHaveHappened();

            A.CallTo(() => _dummyCalculator.Add(A<double>._, A<double>._))
                .Returns(100.0);

            var sum = _dummyCalculator.Add(10.0, 10.0);

            Assert.AreEqual(100.0, sum);
            A.CallTo(() => _dummyCalculator.Add(A<double>._, A<double>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}