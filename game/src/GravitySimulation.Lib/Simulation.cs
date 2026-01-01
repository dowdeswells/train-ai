using System.Diagnostics;

namespace GravitySimulation.Lib;

public class Simulation
{
    private readonly ISimulationController _simulationController;
    private readonly ISimulationTelemetry _simulationTelemetry;
    private readonly IMonitoringService? _monitoringService;
    private readonly Stopwatch _timer = new Stopwatch();
    private ulong _simulationLoop = 0;
    
    public Simulation(ISimulationController simulationController, ISimulationTelemetry simulationTelemetry, IMonitoringService? monitoringService = null)
    {
        _simulationController = simulationController;
        _simulationTelemetry = simulationTelemetry;
        _monitoringService = monitoringService;
    }

    public void Start(WorldModel world)
    {
        var lastTime = ResetTime();

        while (!_simulationController.EndSimulation)
        {
            if (_simulationController.IsReset(_simulationLoop))
            {
                _simulationLoop++;
                world.Reset();
                lastTime = ResetTime();
            }
            
            // 1. Timing Logic (DeltaTime)
            double currentTime = _timer.Elapsed.TotalSeconds;
            double deltaTime = currentTime - lastTime;
            lastTime = currentTime;

            var thing = world._things.First();

    
            world.Advance(deltaTime);

            _simulationTelemetry.WriteTelemetry(currentTime, thing);

            if (_monitoringService != null)
            {
                _monitoringService.Update(world, currentTime, _simulationLoop);
            }

            Thread.Sleep(20); 
        }
    }

    private double ResetTime()
    {
        _timer.Restart();
        return _timer.Elapsed.TotalSeconds;
    }
}