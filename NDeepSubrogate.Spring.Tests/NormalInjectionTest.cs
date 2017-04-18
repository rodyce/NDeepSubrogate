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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDeepSubrogate.Core.Attributes;
using NSubstitute;
using NUnit.Framework;
using Spring.Objects.Factory.Attributes;
using Spring.Testing.NUnit;
using NDeepSubrogate.Core.Tests.SampleClasses;
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
        private readonly ICalculator _dummyCalculator;

        [Test]
        public void ServicesAreInjected()
        {
            Assert.IsNotNull(_dummyCalculator);
        }

        [Test]
        public void SubrogateRestoreTest()
        {
            
            var subrogateScope = new SpringDeepSurrogateScope(this,
                (AbstractApplicationContext)applicationContext);

            var oldCalculator = _dummyCalculator;

            subrogateScope.DeepSubrogate();

            Assert.AreNotSame(oldCalculator, _dummyCalculator);
        }
    }
}
