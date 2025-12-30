using System.Net.Sockets;
using System.Runtime.InteropServices;
using GravitySimulation.Lib;

namespace GravitySimulation.Console;

public class ConsoleTelemetry: ISimulationTelemetry
{
    private readonly string _ip;
    private readonly int _port;
    private UdpClient _client;

    public ConsoleTelemetry(string ip, int port)
    {
        _ip = ip;
        _port = port;
        _client = new UdpClient();
    }
    
    public void WriteTelemetry(double currentTime, FlyingThing thing)
    {
        // var telemetry = new TelemetryData
        // {
        //     Timestamp = currentTime,
        //     ThrustStatus = thing.IsThrusting,
        //     Altitude = thing.Altitude,
        //     Velocity = thing.Velocity,
        //     Acceleration = thing.Acceleration
        // };        
        var telemetry = new TelemetryData
        {
            Timestamp = currentTime,
            ThrustStatus = false,
            Altitude = Convert.ToInt64(thing.Altitude * 100),
            Velocity = Convert.ToInt64(thing.Velocity * 100),
            Acceleration = Convert.ToInt64(thing.Acceleration * 100),
        };
        ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref telemetry, 1));
        _client.Send(byteSpan, _ip, _port);
        
        
        string thrustStatus = telemetry.ThrustStatus ? "THRUST ON " : "          ";
        System.Console.SetCursorPosition(0, 6);
        System.Console.WriteLine($"Seconds: {currentTime:F2}"    );
        System.Console.WriteLine($"Status: {thrustStatus}");
        System.Console.WriteLine($"Altitude: {telemetry.Altitude / 100.0:F2} m    ");
        System.Console.WriteLine($"Velocity: {telemetry.Velocity / 100.0:F2} m/s  ");
        System.Console.WriteLine($"Acceleration: {telemetry.Acceleration / 100.0:F2} m/s/s  ");
    }
}