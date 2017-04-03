using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContextualMocks.Tests.DummyServices;
using NSubstitute;
using NUnit.Framework;
using Spring.Objects.Factory.Attributes;
using Spring.Testing.NUnit;
using ContextualMocks;

namespace ContextualMocks.Tests
{
    public class NormalInjectionTest : AbstractDependencyInjectionSpringContextTests
    {
        public NormalInjectionTest()
        {
            ConfigLocations = new[] {"context.xml"};
        }

        protected override string[] ConfigLocations { get; }

        [Autowired]
        private readonly IDummyServiceA _dummyServiceA;

        [Autowired]
        private readonly ConcreteClassService _concreteClassService;

        [Test]
        public void ServicesAreInjected()
        {
            Assert.IsNotNull(_dummyServiceA);
            Assert.IsNotNull(_concreteClassService);
        }

        [Test]
        public void MockingWorks()
        {
            IDictionary<Type, object> originalObjects = new Dictionary<Type, object>
            {
                {typeof(IDummyServiceA), _dummyServiceA},
                {typeof(ConcreteClassService), _concreteClassService}
            };
            Func<Type, object> originalObjectsFunc = type => originalObjects[type];

            IDictionary<Type, object> replacenentObjects = new Dictionary<Type, object>
            {
                {typeof(IDummyServiceA), Substitute.For<IDummyServiceA>()},
                {typeof(ConcreteClassService), Substitute.For<ConcreteClassService>()}
            };
            Func<Type, object> replacementObjectsFunc = type => replacenentObjects[type];

            ISet<Type> typesToMockSet = new HashSet<Type>()
            {
                typeof(IDummyServiceA), typeof(ConcreteClassService)
            };

            var surrogateScope = new DeepSurrogateScope(this, typesToMockSet, originalObjectsFunc, replacementObjectsFunc);

            surrogateScope.DeepSubrogate();
            Assert.AreSame(replacenentObjects[typeof(IDummyServiceA)], _dummyServiceA);
            Assert.AreSame(replacenentObjects[typeof(ConcreteClassService)], _concreteClassService);

            surrogateScope.DeepRestore();
            Assert.AreSame(originalObjects[typeof(IDummyServiceA)], _dummyServiceA);
            Assert.AreSame(originalObjects[typeof(ConcreteClassService)], _concreteClassService);
        }
    }
}
