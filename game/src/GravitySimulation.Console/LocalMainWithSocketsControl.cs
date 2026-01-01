using GravitySimulation.Lib;

namespace GravitySimulation.Console;

public static class LocalMainWithSocketsControl
{
    public static void Start()
    {
        System.Console.WriteLine("SIMULATION STARTED");
        System.Console.WriteLine("Hold [SPACEBAR] to thrust UP. Press [ENTER] to exit.");
        System.Console.WriteLine("--------------------------------------------------");


        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;
        var socketController = new SocketBasedController();
        var socketListenerTask = Task.Run(() => socketController.StartListeningOnSocket(), token);

        var inputReader = new ConsoleInputRelayToSocket();
        var inputReaderTask = Task.Run(() =>
        {
            while (!socketController.EndSimulation)
            {
                inputReader.Start();
            }
        }, token);

        var world = new WorldModel();
        var thing = new FlyingThing(socketController);
        world.AddThing(thing);

        var sim = new Simulation(socketController, socketController, new ConsoleMonitor());
        sim.Start(world);


        System.Console.WriteLine($"Socket Listener task Status: {socketListenerTask.Status}");
        System.Console.WriteLine($"Input Reader task Status: {inputReaderTask.Status}");
    }
    
    public static void StartWithNoLocalInput()
    {
        System.Console.WriteLine("SIMULATION STARTED");
        System.Console.WriteLine("Hold [SPACEBAR] to thrust UP. Press [ENTER] to exit.");
        System.Console.WriteLine("--------------------------------------------------");


        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;
        var socketController = new SocketBasedController();
        var socketListenerTask = Task.Run(() => socketController.StartListeningOnSocket(), token);
        

        var world = new WorldModel();
        var thing = new FlyingThing(socketController);
        world.AddThing(thing);

        var sim = new Simulation(socketController, socketController, new ConsoleMonitor());
        sim.Start(world);


        System.Console.WriteLine($"Socket Listener task Status: {socketListenerTask.Status}");
    }
    
    public class ConsoleMonitor : IMonitoringService
    {
        public void Update(WorldModel worldModel, double currentTime, ulong simulationLoop)
        {
            var thing = worldModel._things.First();
            string thrustStatus = thing.IsThrusting ? "THRUST ON " : "          ";
            System.Console.SetCursorPosition(0, 7);
            System.Console.WriteLine($"Simulation: {simulationLoop:N1}");
            System.Console.WriteLine($"Seconds: {currentTime:F2}");
            System.Console.WriteLine($"Status: {thrustStatus}");
            System.Console.WriteLine($"Altitude: {thing.Altitude:F2} m    ");
            System.Console.WriteLine($"Velocity: {thing.Velocity:F2} m/s  ");
            System.Console.WriteLine($"Acceleration: {thing.Acceleration:F2} m/s/s  ");
        }
    }
}

