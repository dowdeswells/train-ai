using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using GravitySimulation.Lib;

namespace GravitySimulation.Console;

public class UdpTelemetryReceiver
{
    public void StartListener(int port, ISimulationController simulationController)
    {
        using UdpClient udpServer = new UdpClient(port);
        IPEndPoint remoteEp = new IPEndPoint(IPAddress.Any, port);
        while (!simulationController.EndSimulation)
        {
            byte[] data = udpServer.Receive(ref remoteEp);
            if (data.Length > 0)
            {
                var thing = MemoryMarshal.Read<TelemetryData>(data);
                // 5. Output (Throttled for readability)
                string thrustStatus = thing.ThrustStatus ? "THRUST ON " : "          ";
                System.Console.SetCursorPosition(0, 6);
                System.Console.WriteLine($"Seconds: N/A");
                System.Console.WriteLine($"Status: {thrustStatus}");
                System.Console.WriteLine($"Altitude: {thing.Altitude:F2} m    ");
                System.Console.WriteLine($"Velocity: {thing.Velocity:F2} m/s  ");
                System.Console.WriteLine($"Acceleration: {thing.Acceleration:F2} m/s/s  ");
            }
            
        }
    }
}