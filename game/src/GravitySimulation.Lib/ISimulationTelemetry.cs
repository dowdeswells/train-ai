namespace GravitySimulation.Lib;

public interface ISimulationTelemetry
{
    void WriteTelemetry(double currentTime, FlyingThing thing);
}