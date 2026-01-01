namespace GravitySimulation.Lib;

public interface IMonitoringService
{
    void Update(WorldModel worldModel, double currentTime, ulong simulationLoop);
}