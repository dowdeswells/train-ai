using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using GravitySimulation.Lib;

namespace GravitySimulation.Console;

public class SocketBasedController : IFlightController, ISimulationController, ISimulationTelemetry
{
    private const byte CmdDoNothing = 0;
    private const byte CmdThrustOn = 1;
    private const byte CmdThrustOff = 2;
    private const byte CmdReset = 3;
    private const byte CmdEnd = 4;
    private const string SocketPath = "/tmp/gravity_sim.sock";
    
    
    public bool IsThrusting { get; set; }
    public bool EndSimulation { get; set; }
    private TelemetryData CurrentTelemetry {get; set;}
    private readonly Lock _telemetryLock = new Lock();
    private ulong _currentLoop = 0;
    
    public void WriteTelemetry(double currentTime, FlyingThing thing)
    {
        var telemetry = new TelemetryData
        {
            Timestamp = currentTime,
            ThrustStatus = thing.IsThrusting,
            Altitude = Convert.ToInt64(thing.Altitude * 100),
            Velocity = Convert.ToInt64(thing.Velocity * 100),
            Acceleration = Convert.ToInt64(thing.Acceleration * 100),
        };

        lock (_telemetryLock)
        {
            CurrentTelemetry = telemetry;
        }
    }

    public void StartListeningOnSocket()
    {
        if (File.Exists(SocketPath)) File.Delete(SocketPath);

        var endpoint = new UnixDomainSocketEndPoint(SocketPath);
        using var server = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

        server.Bind(endpoint);
        server.Listen(5);
        while (!EndSimulation)
        {
            HandleConnection(server);
        }

    }
    public bool IsReset(ulong simulationLoop)
    {
        return simulationLoop != _currentLoop;
    }

    private void HandleConnection(Socket server)
    {
        Debug.WriteLine("Server waiting for connection...");
            
        using var handler = server.Accept();
        var handlerConnected = true;
        while (!EndSimulation && handlerConnected)
        {
            handlerConnected = ProcessConnectedHandler(handler);
        }

        if (handlerConnected)
        {
            handler.Close();
        }
    }

    private bool ProcessConnectedHandler(Socket handler)
    {
        try
        {
            var b = ReceiveCommand(handler);
            if (!ProcessCommand(b)) return false;

            Thread.Sleep(100);
            SendTelemetry(handler);
        }
        catch (SocketException e)
        {
            System.Console.WriteLine(e);
            return false;
        }

        return true;
    }

    private bool ProcessCommand(byte? b)
    {
        if (b == null)
        {
            return false;
        }

        switch (b)
        {
            case CmdThrustOn:
                IsThrusting = true;
                break;
            case CmdThrustOff:
                IsThrusting = false;
                break;
            case CmdReset:
                Interlocked.Increment(ref _currentLoop);
                break;
            case CmdEnd:
                EndSimulation = true;
                break;
            case CmdDoNothing:
                break;
            default:
                break;
        }

        return true;
    }

    private void SendTelemetry(Socket handler)
    {
        ReadOnlySpan<byte> byteSpan;
        TelemetryData telemetry;
        lock (_telemetryLock)
        {
            telemetry = CurrentTelemetry;
            byteSpan = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref telemetry, 1));
        }
            
        handler.Send(byteSpan);
    }

    private static byte? ReceiveCommand(Socket handler)
    {
        byte[] buffer = new byte[1];
        int received = handler.Receive(buffer);
        if (received == 0)
            return null;
        var b = buffer[0];
        return b;
    }
}