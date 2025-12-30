using System.Diagnostics;

namespace GravitySimulation.Lib;

public class Simulation
{
    private readonly ISimulationController _simulationController;
    private readonly ISimulationTelemetry _simulationTelemetry;
    private readonly Stopwatch _timer = new Stopwatch();

    public Simulation(ISimulationController simulationController, ISimulationTelemetry simulationTelemetry)
    {
        _simulationController = simulationController;
        _simulationTelemetry = simulationTelemetry;
        
    }

    public void Start(WorldModel world)
    {
        var lastTime = ResetTime();

        while (!_simulationController.EndSimulation)
        {
            if (_simulationController.Reset)
            {
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

            Thread.Sleep(20); // Run at ~50fps
        }
    }

    private double ResetTime()
    {
        _timer.Restart();
        return _timer.Elapsed.TotalSeconds;
    }
}