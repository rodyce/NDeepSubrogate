﻿using NDeepSubrogate.Core.Tests.SampleClasses;
using NDeepSubrogate.Core.Tests.SampleInterfaces;
using Spring.Objects.Factory.Attributes;
using Spring.Stereotype;

namespace NDeepSubrogate.Spring.Tests.ServiceImpl
{
    [Component]
    public class SpringAirplane : Airplane
    {
        [Autowired]
        private readonly ICalculator _calculator = null;

        protected override ICalculator Calculator => _calculator;
    }
}
