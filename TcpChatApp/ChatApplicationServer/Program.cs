using System.Net.Sockets;
using System.Text;
using System.Text.Json;

TcpListener server = CreateServer.GetTcpListener();
server.Start();
Console.WriteLine("Server started. Waiting for connections \r\n");

try
{
    while (true)
    {
        while (true)
        {
            TcpClient client = await server.AcceptTcpClientAsync();
            _ = HandleClientAsync(client);
        }
    }
} finally
{
    server.Stop();
}
 

static async Task HandleClientAsync(TcpClient client)
{
    Console.WriteLine("Client {0} connected\r\n", client.Client.RemoteEndPoint.ToString());
    await GoToRoomAsync(1, client, client.GetStream());
}

static async Task GoToRoomAsync(int roomNumber, TcpClient client, NetworkStream stream)
{
    RoomActions.AddToRoom(client, roomNumber);

    try
    {
        while (true)
        {
            byte[] buffer = new byte[1024]; // Adjust buffer size as needed
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead == 0)
            {
                break;
            }

            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            if (message.TrimStart().StartsWith("{"))
            {
                try
                {
                    var deserializedMessage = JsonSerializer.Deserialize<HttpMessage>(message);
                    RoomActions.PostToRoom(client, roomNumber, deserializedMessage!);
                } catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            } else
            {
                int bodyStartIndex = message.IndexOf("\r\n\r\n") + 4;

                if (bodyStartIndex >= 0 && bodyStartIndex < message.Length)
                {
                    try
                    {
                        string body = message.Substring(bodyStartIndex);
                        var deserializedMessage = JsonSerializer.Deserialize<HttpMessage>(body);
                        RoomActions.PostToRoom(client, roomNumber, deserializedMessage!);
                        Console.WriteLine("Received HTTP Request Body:\n" + body);
                    } catch (Exception e)
                    {
                        Console.WriteLine("Failed to extract body, " + e.Message);
                    }
                }
                else
                {
                    Console.WriteLine("HTTP Request does not contain a body.");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
    finally
    {
        RoomActions.RemoveFromRoom(client, roomNumber);
        client.Close();
        Console.WriteLine("Client disconnected");
    }
}