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


using NDeepSubrogate.Core.Tests.SampleInterfaces;
using NDeepSubrogate.Spring.Tests.ServiceImpl;
using NUnit.Framework;
using Spring.Context.Support;
using Spring.Objects.Factory.Attributes;
using Spring.Objects.Factory.Support;
using Spring.Testing.NUnit;

namespace NDeepSubrogate.Spring.Tests
{
    internal class FactoryObjectTests : AbstractDependencyInjectionSpringContextTests
    {
        public FactoryObjectTests()
        {
            ConfigLocations = new[] { "context.xml" };
        }

        protected override string[] ConfigLocations { get; }

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
        public void ObjectDefinitionReplacementTest()
        {
            var appContext = (AbstractApplicationContext)applicationContext;
            var objectDefinitionName = _dummyCalculator.GetType().FullName;

            var originalObjectDefinition = appContext.GetObjectDefinition(objectDefinitionName);
            var surrogateObjectDefinition = ObjectDefinitionBuilder.GenericObjectDefinition(
                typeof(SurrogateFactoryObject))
                .AddPropertyValue("TargetObjectType", typeof(ICalculator))
                .ObjectDefinition;

            appContext.ReplaceObjectDefinition(objectDefinitionName, surrogateObjectDefinition);

            var surrogateObject = appContext.GetObject(objectDefinitionName);

            Assert.AreNotSame(surrogateObject.GetType(), _dummyCalculator.GetType());

            //Replace
            appContext.ReplaceObjectDefinition(objectDefinitionName, originalObjectDefinition);

            var originalObject = appContext.GetObject(objectDefinitionName);

            Assert.AreNotSame(surrogateObject, originalObject);
            Assert.AreSame(_dummyCalculator.GetType(), originalObject.GetType());
        }

    }
}
