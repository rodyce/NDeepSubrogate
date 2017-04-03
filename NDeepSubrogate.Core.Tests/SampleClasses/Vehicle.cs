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
