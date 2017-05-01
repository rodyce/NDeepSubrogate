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

        [Subrogate]
        [Autowired]
        private readonly SpringVehicle _vehicle = null;

        [Test]
        public void ServicesAreInjected()
        {
            Assert.IsNotNull(_vehicle);
        }

        [Test]
        public void RestoreTest()
        {
            var subrogateScope = new SpringDeepSurrogateScope(this,
                (AbstractApplicationContext)applicationContext);

            var oldVehicle = _vehicle;

            subrogateScope.DeepSubrogate();

            Assert.AreNotSame(oldVehicle.GetType(), _vehicle.GetType());

            subrogateScope.DeepRestore();

            Assert.AreSame(oldVehicle.GetType(), _vehicle.GetType());
        }
    }
}