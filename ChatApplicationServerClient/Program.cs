using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ChatApplicationServerClient
{
    public class Program
    {
        public static async Task Main()
        {
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

                Task readTask = ReadFromServerAsync(stream);
                Task writeTask = WriteToServerAsync(stream, username);

                // Wait for both read and write tasks to complete
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
                    await Task.Delay(100); // Add a delay to avoid busy-waiting
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
                    await stream.FlushAsync(); // Flush the stream to ensure data is sent immediately
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error writing to server: {0}", e.Message);
            }
        }

        /*public static void Main()
        {
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
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }*/
    }
}