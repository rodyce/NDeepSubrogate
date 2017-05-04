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
using NUnit.Framework;
using Spring.Objects.Factory.Attributes;
using Spring.Testing.NUnit;
using NDeepSubrogate.Core.Tests.SampleInterfaces;
using NDeepSubrogate.Spring.Tests.ServiceImpl;
using Spring.Context.Support;

namespace NDeepSubrogate.Spring.Tests
{
    [DeepSubrogate]
    public class DeepSubrogationTests : AbstractDependencyInjectionSpringContextTests
    {
        public DeepSubrogationTests()
        {
            ConfigLocations = new[] {"context.xml"};
        }

        protected override string[] ConfigLocations { get; }

        [Subrogate]
        [Autowired]
        private readonly ICalculator _dummyCalculator = null;

        [Autowired]
        private readonly SpringVehicle _vehicle = null;

        [Test]
        public void ServicesAreInjected()
        {
            Assert.IsNotNull(_dummyCalculator);
            Assert.IsNotNull(_vehicle);
        }

        [Test]
        public void BeforeSubrogateTest()
        {
            _vehicle.Accelerate(10);
            _vehicle.Accelerate(30);

            Assert.AreEqual(40, _vehicle.SpeedInKph);
        }

        [Test]
        public void RestoreTest()
        {
            var subrogateScope = new SpringDeepSurrogateScope(this,
                (AbstractApplicationContext)applicationContext);

            var oldCalculator = _dummyCalculator;

            subrogateScope.DeepSubrogate();

            Assert.AreNotSame(oldCalculator.GetType(), _dummyCalculator.GetType());

            subrogateScope.DeepRestore();

            Assert.AreSame(oldCalculator.GetType(), _dummyCalculator.GetType());
        }

        [Test]
        public void DeepSubrogateTest()
        {
            var subrogateScope = new SpringDeepSurrogateScope(this,
                (AbstractApplicationContext)applicationContext);

            // Subrogate the referenced objects as marked with the [Subrogate] annotation.
            subrogateScope.DeepSubrogate();

            // Now that the original objects have been subrogates with fakes / mocks,
            // configure those. In this case, configure the Add method of ICalculator
            // to always return 100, no matter the arguments.
            A.CallTo(() => _dummyCalculator.Add(A<double>._, A<double>._)).Returns(100.0);

            // Make sure vehicle's speed is 0.0 Km/h (stopped)
            _vehicle.Stop();
            Assert.AreEqual(0, _vehicle.SpeedInKph);

            // Accelerate the vehicle by 10 and then 40 Km/h.
            _vehicle.Accelerate(10);
            _vehicle.Accelerate(40);
            // Expected result here is 100.0, instead of 50.0. This is because every time we add
            // two doubles, the result is always 100.0, as configured before.
            Assert.AreEqual(100, _vehicle.SpeedInKph);

            // Restore the original objects that were subrogated earlier.
            subrogateScope.DeepRestore();

            // Make sure vehicle's speed is 0.0 Km/h (stopped)
            _vehicle.Stop();
            Assert.AreEqual(0, _vehicle.SpeedInKph);

            // Accelerate the vehicle by 10 and then 40 Km/h.
            _vehicle.Accelerate(10);
            _vehicle.Accelerate(40);
            // Expected result here is 50.0 This is because the original ICalculator object has been
            // restored and does the correct math.
            Assert.AreEqual(50, _vehicle.SpeedInKph);
        }
    }
}
