using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
    Console.WriteLine("Client connected\r\n");
    await GoToRoomAsync(1, client, client.GetStream());
}

static async Task GoToRoomAsync(int? roomNumber, TcpClient client, NetworkStream stream)
{
    int room = (int)roomNumber;
    RoomActions.AddToRoom(client, room);

    Console.WriteLine("Hello");

    try
    {
        while (true)
        {
            byte[] buffer = new byte[1024]; // Adjust buffer size as needed
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead == 0)
            {
                // Client disconnected
                break;
            }

            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            var deserializedMessage = JsonSerializer.Deserialize<HttpMessage>(message);

            RoomActions.PostToRoom(client, room, deserializedMessage!);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
    finally
    {
        RoomActions.RemoveFromRoom(client, (int)roomNumber);
        client.Close();
        Console.WriteLine("Client disconnected");
    }
}