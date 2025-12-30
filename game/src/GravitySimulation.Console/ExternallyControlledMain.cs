using GravitySimulation.Lib;

namespace GravitySimulation.Console;

public static class ExternallyControlledMain
{
    public static void Start()
    {
        System.Console.WriteLine("SIMULATION STARTED");
        System.Console.WriteLine("Hold [SPACEBAR] to thrust UP. Press [ENTER] to exit.");
        System.Console.WriteLine("--------------------------------------------------");
        
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;
        var consoleTelemetry = new ConsoleTelemetry("127.0.0.1", Constants.SimulationTelemetryUdpPortNo);

        var udpController = new UdpController();
        var controllerTask = Task.Run(() => udpController.StartListener(Constants.FlightControllerUdpPortNo), token);

        var world = new WorldModel();
        var thing = new FlyingThing(udpController);
        world.AddThing(thing);

        var sim = new Simulation(udpController, consoleTelemetry);
        sim.Start(world);
        
        System.Console.WriteLine($"UDP task Status: {controllerTask.Status}");
    }
}