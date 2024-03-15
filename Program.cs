namespace ChatApplication;
using System.Net.Sockets;
using System.Net;

class Program
{
    static void Main(string[] args)
    {
        var localAddress = "";
        int port = 8081;

        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                System.Console.WriteLine(ip.ToString());
                localAddress = ip.ToString();
            }
        }

        TcpListener server = new(IPAddress.Parse(localAddress), port);
        server.Start();

        Console.WriteLine("Server started on {0}:{1}\r\n. Waiting for connection...", localAddress, port);

        server.Stop();
    }
}

