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
    public class NormalInjectionTest : AbstractDependencyInjectionSpringContextTests
    {
        public NormalInjectionTest()
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

            Assert.AreNotSame(oldCalculator, _dummyCalculator);

            subrogateScope.DeepRestore();

            Assert.AreSame(oldCalculator, _dummyCalculator);
        }

        /*
        [Test]
        public void DeepSubrogateTest()
        {
            var subrogateScope = new SpringDeepSurrogateScope(this,
                (AbstractApplicationContext)applicationContext);

            subrogateScope.DeepSubrogate();

            _dummyCalculator.Add(Arg.Any<double>(), Arg.Any<double>()).Returns(100.0);

            _vehicle.Accelerate(10);
            _vehicle.Accelerate(40);
            Assert.AreEqual(100, _vehicle.SpeedInKph);

            subrogateScope.DeepRestore();
        }*/
    }
}
