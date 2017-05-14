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
using NDeepSubrogate.Core.Tests.SampleClasses;
using NDeepSubrogate.Core.Tests.SampleInterfaces;
using NDeepSubrogate.Core.Tests.SampleInterfaces.Impl;

namespace NDeepSubrogate.Core.Tests.TestUtils
{
    internal class DummyNoSurrogate
    {
        private readonly ICalculator _calculator = new Calculator();
        private readonly Vehicle _vehicle = new Vehicle();

        public ICalculator Calculator => _calculator;
        public Vehicle Vehicle => _vehicle;
    }

    [DeepSubrogate(Enabled = false)]
    internal class DummySubrogationDisabled
    {
        [Subrogate]
        private readonly ICalculator _calculator = new Calculator();
        [Subrogate]
        private readonly Vehicle _vehicle = new Vehicle();

        public ICalculator Calculator => _calculator;
        public Vehicle Vehicle => _vehicle;
    }

    [DeepSubrogate]
    internal class DummyOneSurrogate
    {
        [Subrogate]
        private readonly ICalculator _calculator = new Calculator();

        private readonly Vehicle _vehicle = new Vehicle();

        public ICalculator Calculator => _calculator;
        public Vehicle Vehicle => _vehicle;
    }

    [DeepSubrogate]
    internal class DummyTwoSurrogates
    {
        [Subrogate]
        private readonly ICalculator _calculator = new Calculator();
        [Subrogate]
        private readonly Vehicle _vehicle = new Vehicle();

        public ICalculator Calculator => _calculator;
        public Vehicle Vehicle => _vehicle;
    }
}
