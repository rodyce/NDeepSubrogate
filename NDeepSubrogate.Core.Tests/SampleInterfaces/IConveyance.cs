namespace NDeepSubrogate.Core.Tests.SampleInterfaces
{
    public interface IConveyance
    {
        void Accelerate(int speed);

        int TimeToArriveInSecs(int distanceInKm);

        void Stop();

        int SpeedInKph { get; }
    }
}
