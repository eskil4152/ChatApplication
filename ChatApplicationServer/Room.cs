using System;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;

public struct Room
{
	public string RoomName { get; set; }
    public int RoomNumber { get; set; }
    public string RoomPassword { get; set; }
    public string RoomSecret { get; set; }
    public List<TcpClient> Members { get; set; }
}

public static class RoomActions
{
    private static readonly Dictionary<int, Room> rooms = new();

    public static void RemoveFromRoom(TcpClient client, int roomNumber)
    {
        Room room = rooms[roomNumber];
        room.Members.Remove(client);
    }

    public static void AddToRoom(TcpClient client, int roomNumber)
    {
        if (rooms.ContainsKey(roomNumber))
        {
            Room room = rooms[roomNumber];
            room.Members ??= new List<TcpClient>();

            rooms[roomNumber].Members.Add(client);
        }
        else
        {
            Room newRoom = new()
            {
                RoomName = "",
                RoomPassword = "",
                RoomSecret = ""
            };
            newRoom.Members = new List<TcpClient>
            {
                client
            };

            rooms.Add(roomNumber, newRoom);
        }

        Console.WriteLine("ADDED TO ROOM");
    }

    public static Room GetRoom(int roomNumber)
    {
        return rooms[roomNumber];
    }

    public static void PostToRoom(TcpClient sender, int roomNumber, HttpMessage message)
    {
        Console.WriteLine("POSTED");
        UpdateRoom(sender, roomNumber, message.Username, message.Message);
    }

    public static void UpdateRoom(TcpClient sender, int roomNumber, string user, string message)
    {
        Room room = rooms[roomNumber];
        Console.WriteLine("Got room");

        foreach (TcpClient client in room.Members)
        {
            if (client != sender)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    byte[] data = Encoding.UTF8.GetBytes(user + ": " + message);
                    stream.Write(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error broadcasting message to client: {ex.Message}");
                }
            }
        }
    }
}
