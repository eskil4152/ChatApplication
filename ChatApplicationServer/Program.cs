using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ChatApplicationServer
{
    public class Program
    {
        public static async void Main()
        {
            TcpListener server = CreateServer.GetTcpListener();
            server.Start();

            Console.WriteLine("Server started. Waiting for connections \r\n");

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                System.Console.WriteLine("Client connected\r\n");

                NetworkStream stream = client.GetStream();

                while (!stream.DataAvailable) ;
                while (client.Available < 3) ;

                byte[] bytes = new byte[client.Available];
                stream.Read(bytes, 0, bytes.Length);
                string s = Encoding.UTF8.GetString(bytes);

                Console.WriteLine("Whole string: {0}", s);
                var res = JsonSerializer.Deserialize<HttpMessage>(s);

                if (res?.RoomNumber == null)
                {
                    Console.WriteLine("Room number was null");
                }

                Console.WriteLine("Client with username {0} wrote:'{1}' for room {2}", res?.Username, res?.Message, res?.RoomNumber);

                string response = "Received";
                byte[] data = Encoding.ASCII.GetBytes(response);
                stream.Write(data, 0, data.Length);
            }
        }
    }
}