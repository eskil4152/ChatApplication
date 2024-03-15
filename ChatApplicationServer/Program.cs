using System.Net;
using System.Net.Sockets;

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

Console.WriteLine("Server started on {0}:{1}.\r\n Waiting for connection...\r\n", localAddress, port);

TcpClient client = server.AcceptTcpClient();

System.Console.WriteLine("Client connected\r\n");

server.Stop();