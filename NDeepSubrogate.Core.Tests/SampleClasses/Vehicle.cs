﻿#region License

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
using NDeepSubrogate.Core.Tests.SampleInterfaces.Impl;

namespace NDeepSubrogate.Core.Tests.SampleClasses
{
    public class Vehicle : IConveyance
    {
        private ICalculator _calculator;

        public Vehicle()
        {
            SpeedInKph = 0;
        }

        public void Accelerate(int speed)
        {
            SpeedInKph = (int)Calculator.Add(SpeedInKph, speed);
        }

        public int TimeToArriveInSecs(int distanceInKm)
        {
            return (int)Calculator.Divide(distanceInKm, SpeedInKph/60.0);
        }

        public void Stop()
        {
            SpeedInKph = 0;
        }

        public int SpeedInKph { get; private set; }

        public virtual ICalculator Calculator => _calculator ?? (_calculator = new Calculator());
    }
}
