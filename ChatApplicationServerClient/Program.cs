using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ChatApplicationServerClient
{
    class Program
    {
        public static async Task Main()
        {
            var hostIp = "192.168.0.135";
            int hostPort = 8081;

            Console.WriteLine("Username:");
            string? username = Console.ReadLine();

            try
            {
                TcpClient client = new(hostIp, hostPort);
                Console.WriteLine("Client connected");

                NetworkStream stream = client.GetStream();

                Task readTask = ReadFromServerAsync(stream);
                Task writeTask = WriteToServerAsync(stream, username);

                await Task.WhenAll(readTask, writeTask);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }

        private static async Task ReadFromServerAsync(NetworkStream stream)
        {
            try
            {
                while (true)
                {
                    if (stream.DataAvailable)
                    {
                        byte[] data = new byte[256];
                        int bytes = await stream.ReadAsync(data, 0, data.Length);
                        string responseData = Encoding.ASCII.GetString(data, 0, bytes);
                        Console.WriteLine(responseData);
                    }
                    await Task.Delay(100);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading from server: {0}", e.Message);
            }
        }

        private static async Task WriteToServerAsync(NetworkStream stream, string? username)
        {
            try
            {
                while (true)
                {
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

                    await stream.WriteAsync(byteArray, 0, byteArray.Length);
                    await stream.FlushAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error writing to server: {0}", e.Message);
            }
        }
    }
}