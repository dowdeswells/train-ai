using System.Net.Sockets;
using System.Runtime.InteropServices;
using GravitySimulation.Lib;

namespace GravitySimulation.Console;

public class ConsoleInputRelayToSocket
{
    private const byte CmdDoNothing = 0;
    private const byte CmdThrustOn = 1;
    private const byte CmdThrustOff = 2;
    private const byte CmdReset = 3;
    private const byte CmdEnd = 4;
    private const string SocketPath = "/tmp/gravity_sim.sock";

    
    public void Start()
    {
        var endpoint = new UnixDomainSocketEndPoint(SocketPath);
        using var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
        socket.Connect(endpoint);
        using var stream = new NetworkStream(socket);
        
        var endSimulation = false;
        var thrustOn = false;
        while (!endSimulation)
        {
            if (System.Console.KeyAvailable)
            {
                var data = GetCommand(ref thrustOn);

                if (data != CmdDoNothing)
                {
                    SendCommand(stream, data);
                }
                endSimulation = data == CmdEnd;
            }
            else
            {
                SendCommand(stream, CmdDoNothing);
            }
            Thread.Sleep(100);
        }

        
    }

    private static void SendCommand(NetworkStream stream, byte data)
    {
        try
        {
            stream.WriteByte(data);
                        
            byte[] responseBuffer = new byte[64];
            int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);

            if (bytesRead > 0)
            {
                //OutputTelemetry(responseBuffer);
            }
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            throw;
        }
    }

    private static void OutputTelemetry(byte[] responseBuffer)
    {
        var thing = MemoryMarshal.Read<TelemetryData>(responseBuffer);
        // 5. Output (Throttled for readability)
        string thrustStatus = thing.ThrustStatus ? "THRUST ON " : "          ";
        System.Console.SetCursorPosition(0, 6);
        System.Console.WriteLine($"Seconds: {thing.Timestamp:F2}");
        System.Console.WriteLine($"Status: {thrustStatus}");
        System.Console.WriteLine($"Altitude: {thing.Altitude/100.0:F2} m    ");
        System.Console.WriteLine($"Velocity: {thing.Velocity/100.0:F2} m/s  ");
        System.Console.WriteLine($"Acceleration: {thing.Acceleration/100.0:F2} m/s/s  ");
    }

    private static byte GetCommand(ref bool thrustOn)
    {
        byte data = CmdDoNothing;
        var key = System.Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.Escape:
                data = CmdEnd;
                break;
            case ConsoleKey.Spacebar:
                thrustOn = !thrustOn;
                data = thrustOn ? CmdThrustOn : CmdThrustOff;
                break;
            case ConsoleKey.Enter:
                data = CmdReset;
                break;
            default:
                data = CmdDoNothing;
                break;
        }

        return data;
    }
}