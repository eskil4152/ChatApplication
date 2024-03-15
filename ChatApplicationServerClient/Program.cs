using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

var hostIp = "192.168.0.135";
int hostPort = 8081;

Console.WriteLine("Username:");
string? username = Console.ReadLine();

try
{
    // Establish connection to server
    TcpClient client = new(hostIp, hostPort); 
    Console.WriteLine("Client connected");

    NetworkStream stream = client.GetStream();

    Thread readThread = new(() =>
    {
        while (true)
        {
            if (stream.DataAvailable)
            {
                byte[] data = new byte[256];
                int bytes = stream.Read(data, 0, data.Length);
                string responseData = Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Server response: " + responseData);
            }
        }
    });
    readThread.Start();

    // Thread for writing messages to the server
    Thread writeThread = new(() =>
    {
        while (true)
        {
            Console.WriteLine("What to write?");
            string? messageInput = Console.ReadLine();

            HttpMessage message = new()
            {
                Username = username,
                Password = "password",
                RoomNumber = 1,
                Message = messageInput
            };

            string json = JsonSerializer.Serialize(message);
            byte[] byteArray = Encoding.ASCII.GetBytes(json);

            stream.Write(byteArray, 0, byteArray.Length);
        }
    });
    writeThread.Start();

    /*while (true)
    {
        while (!stream.DataAvailable);

        // Receive Response
        byte[] data = new byte[256];
        StringBuilder responseData = new();
        int bytes = stream.Read(data, 0, data.Length);
        responseData.Append(Encoding.ASCII.GetString(data, 0, bytes));
        Console.WriteLine("Server response: " + responseData.ToString());

        // Send data
        Console.WriteLine("What to write back?");
        message = Console.ReadLine() + "";
        string json = JsonSerializer.Serialize(messageObject);
        byte[] byteArray = Encoding.ASCII.GetBytes(json);
        stream.Write(byteArray, 0, byteArray.Length);
    }*/
}
catch (Exception e)
{
    Console.WriteLine("Error: {0}", e.Message);
}