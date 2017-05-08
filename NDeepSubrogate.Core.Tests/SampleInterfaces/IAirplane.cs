namespace NDeepSubrogate.Core.Tests.SampleInterfaces
{
    public interface IAirplane : IConveyance
    {
        void Elevate(double meters);
        double Altitude { get; }
        double MachNumber { get; }
    }
}
