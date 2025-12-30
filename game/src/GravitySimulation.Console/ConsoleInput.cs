using System.Net.Sockets;
using GravitySimulation.Lib;

namespace GravitySimulation.Console;

public class ConsoleInput : IInputReader
{
    private UdpClient _client;

    
    public void Read()
    {
        if (System.Console.KeyAvailable)
        {
            var key = System.Console.ReadKey(true).Key;
            var data = key switch
            {
                ConsoleKey.Escape => Constants.StopGame,
                ConsoleKey.Enter => Constants.Reset,
                ConsoleKey.Spacebar => Constants.Thrust,
                _ => Constants.None
            };
            _client.Send([data]);
        }
        else
        {
            _client.Send([Constants.None]);
        }
    }

    public void StartAsUdp(string ip, int port)
    {
        _client = new UdpClient();
        _client.Connect(ip, port); 
    }
    

}