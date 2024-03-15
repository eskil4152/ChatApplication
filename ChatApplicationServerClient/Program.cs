using System.Net;
using System.Net.Sockets;

var hostIp = "192.168.0.135";
int hostPort = 8081;

try
{
    TcpClient client = new(hostIp, hostPort);

    Console.WriteLine("Client connected");
}
catch
{
    Console.WriteLine("Error");
}