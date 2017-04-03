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
using NDeepSubrogate.Core.Tests.SampleInterfaces;
using NDeepSubrogate.Core.Tests.SampleInterfaces.Impl;

namespace NDeepSubrogate.Core.Tests.SampleClasses
{
    public class Vehicle
    {
        private readonly ICalculator _calculator = new Calculator();

        public Vehicle()
        {
            SpeedInKph = 0;
            Color = ConsoleColor.Blue;
        }

        public void Accelerate(int speed)
        {
            SpeedInKph = (int) _calculator.Add(SpeedInKph, speed);
        }

        public int TimeToArriveInSecs(int distanceInKm)
        {
            return (int)_calculator.Divide(distanceInKm, SpeedInKph/60.0);
        }

        public ConsoleColor Color { get; set; }

        public int SpeedInKph { get; private set; }
    }
}
