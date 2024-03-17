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
            Console.WriteLine("Username:");
            string username = VerifyInputString();

            Console.WriteLine("Which room do you want to enter?");
            int room = VerifyInputInteger();

            var hostIp = "";
            int hostPort = 8081;

            try
            {
                TcpClient client = new(hostIp, hostPort);
                Console.WriteLine("Client connected");

                NetworkStream stream = client.GetStream();

                Task readTask = ReadFromServerAsync(stream);
                Task writeTask = WriteToServerAsync(stream, username, room);

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

        private static async Task WriteToServerAsync(NetworkStream stream, string username, int room)
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
                        RoomNumber = room,
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

        public static string VerifyInputString()
        {
            while (true)
            {
                string? input = Console.ReadLine();

                if (!String.IsNullOrEmpty(input))
                {
                    return input;
                }

                Console.WriteLine("Please enter a valid input");
            }
        }

        public static int VerifyInputInteger()
        {
            while (true)
            {
                string? input = Console.ReadLine();
                                
                if (!String.IsNullOrEmpty(input) && int.TryParse(input, out int output) )
                {
                    return output;    
                }

                Console.WriteLine("Please enter a valid number");
            }
        }
    }
}