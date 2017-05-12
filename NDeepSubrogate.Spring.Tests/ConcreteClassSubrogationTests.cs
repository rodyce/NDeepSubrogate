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
using NDeepSubrogate.Spring.Tests.ServiceImpl;
using NUnit.Framework;
using Spring.Context.Support;
using Spring.Objects.Factory.Attributes;
using Spring.Testing.NUnit;

namespace NDeepSubrogate.Spring.Tests
{
    [DeepSubrogate]
    public class ConcreteClassSubrogationTests : AbstractDependencyInjectionSpringContextTests
    {
        public ConcreteClassSubrogationTests()
        {
            ConfigLocations = new[] { "context.xml" };
        }

        protected override string[] ConfigLocations { get; }

        [Subrogate(Spy = true)]
        [Autowired]
        private readonly ICalculator _calculator = null;

        [Autowired]
        private readonly SpringVehicle _vehicle = null;

        [Subrogate(Spy = true)]
        [Autowired]
        private readonly SpringAirplane _airplane = null;

        [Test]
        public void SpiesAreApplied()
        {
            var subrogateScope = new SpringDeepSurrogateScope(this,
                (AbstractApplicationContext)applicationContext);

            subrogateScope.DeepSubrogate();

            Assert.AreEqual(13, _calculator.Add(7, 6));
            A.CallTo(() => _calculator.Add(A<double>._, A<double>._))
                .MustHaveHappened(Repeated.Exactly.Once);
            Assert.IsNotNull(_vehicle);

            subrogateScope.DeepRestore();
        }

        [Test]
        public void SubrogateAndRestoreContextTest()
        {
            var subrogateScope = new SpringDeepSurrogateScope(this,
                (AbstractApplicationContext)applicationContext);

            var oldCalculator = _calculator;
            var oldAirplane = _airplane;

            subrogateScope.DeepSubrogate();

            Assert.AreNotSame(oldCalculator.GetType(), _calculator.GetType());
            Assert.AreNotSame(oldAirplane.GetType(), _airplane.GetType());

            subrogateScope.DeepRestore();

            Assert.AreSame(oldCalculator.GetType(), _calculator.GetType());
            Assert.AreSame(oldAirplane.GetType(), _airplane.GetType());
        }

        [Test]
        public void SpySurrogateTest()
        {
            var subrogateScope = new SpringDeepSurrogateScope(this,
                (AbstractApplicationContext)applicationContext);
            try
            {
                subrogateScope.DeepSubrogate();

                _vehicle.Accelerate(50);
                _vehicle.Accelerate(150);

                _airplane.Accelerate(500);
                _airplane.Accelerate(1500);

                Assert.AreEqual(200.0, _vehicle.SpeedInKph);
                Assert.AreEqual(2000.0, _airplane.SpeedInKph);
                A.CallTo(() => _calculator.Add(A<double>._, A<double>._))
                    .MustHaveHappened(Repeated.Exactly.Times(4));
            }
            finally
            {
                subrogateScope.DeepRestore();
            }
        }
    }
}