using System;
using System.Net;
using System.Net.Sockets;

public class CreateServer
{
    public static TcpListener GetTcpListener()
    {
        var localAddress = "";
        int port = 8081;

        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localAddress = ip.ToString();
            }
        }

        TcpListener server = new(IPAddress.Parse(localAddress), port);

        Console.WriteLine("Server created on {0}:{1}\r\n", localAddress, port);
        return server;
    }
}