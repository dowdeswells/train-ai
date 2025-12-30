using System.Net;
using System.Net.Sockets;
using System.Text;
using GravitySimulation.Lib;

namespace GravitySimulation.Console;

public class UdpController : IFlightController, ISimulationController
{
    public bool IsThrusting { get; set; }
    public bool Reset { get; set; }
    public bool EndSimulation { get; set; }
    
    public void StartListener(int port)
    {
        using UdpClient udpServer = new UdpClient(port);
        udpServer.Client.ReceiveTimeout = 20;
        IPEndPoint remoteEp = new IPEndPoint(IPAddress.Any, port);
        while (!EndSimulation)
        {
            var first = Receive(udpServer, remoteEp);
            IsThrusting = first == Constants.Thrust;
            EndSimulation = first == Constants.StopGame;
            Reset = first == Constants.Reset;
        }
    }

    private static byte Receive(UdpClient udpServer, IPEndPoint remoteEp)
    {
        try
        {
            byte[] data = udpServer.Receive(ref remoteEp);
            return data.Length > 0 ? data[0] : Constants.None;
        }
        catch (SocketException e)
        {
            return Constants.None;
        }

    }
}