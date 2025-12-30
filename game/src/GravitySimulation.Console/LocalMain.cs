using GravitySimulation.Lib;

namespace GravitySimulation.Console;

public static class LocalMain
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

        // var telemetryReceiver = new UdpTelemetryReceiver();
        // var telemetryTask =
        //     Task.Run(() => telemetryReceiver.StartListener(Constants.SimulationTelemetryUdpPortNo, udpController),
        //         token);

        var inputReader = new ConsoleInput();
        inputReader.StartAsUdp("127.0.0.1", Constants.FlightControllerUdpPortNo);
        var inputReaderTask = Task.Run(async () =>
        {
            while (!udpController.EndSimulation)
            {
                inputReader.Read();
                await Task.Delay(20);
            }
        }, token);

        var world = new WorldModel();
        var thing = new FlyingThing(udpController);
        world.AddThing(thing);

        var sim = new Simulation(udpController, consoleTelemetry);
        sim.Start(world);


//await Task.WhenAll(controllerTask, telemetryTask, inputReaderTask);
        System.Console.WriteLine($"UDP task Status: {controllerTask.Status}");
        //System.Console.WriteLine($"Telemetry task Status: {telemetryTask.Status}");
        System.Console.WriteLine($"Input Reader task Status: {inputReaderTask.Status}");
    }
}