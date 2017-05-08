using NDeepSubrogate.Core.Tests.SampleInterfaces;

namespace NDeepSubrogate.Core.Tests.SampleClasses
{
    public class Airplane : IAirplane
    {
        public Airplane()
        {
            SpeedInKph = 0;
            Altitude = 0;
        }
        public void Accelerate(int speed)
        {
            SpeedInKph = (int)Calculator.Add(SpeedInKph, speed);
        }

        public int TimeToArriveInSecs(int distanceInKm)
        {
            return (int)Calculator.Divide(distanceInKm, SpeedInKph / 60.0);
        }

        public void Stop()
        {
            SpeedInKph = 0;
        }

        public int SpeedInKph { get; private set; }

        public void Elevate(double meters)
        {
            Altitude = Calculator.Add(Altitude, meters);
        }

        public double Altitude { get; private set; }

        public double MachNumber => SpeedInKph/300000.0;

        protected virtual ICalculator Calculator { get; set; }
    }
}
