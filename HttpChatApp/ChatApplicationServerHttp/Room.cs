using System.Net.WebSockets;
using System.Text;

namespace ChatApplicationServerHttp
{
    public struct Room
    {
        public string RoomName { get; set; }
        public int RoomNumber { get; set; }
        public string RoomPassword { get; set; }
        public string RoomSecret { get; set; }
        public List<WebSocket> Members { get; set; }
        public List<string> Messages { get; set; }
    }

    public static class RoomActions
    {
        private static readonly Dictionary<int, Room> rooms = new();

        public static void RemoveFromRoom(WebSocket client, int roomNumber)
        {
            Room room = rooms[roomNumber];
            room.Members.Remove(client);
        }

        public static async Task UpdateUsersMessagesAsync(WebSocket client, Room room)
        {
            try
            {
                foreach (string msg in room.Messages)
                {
                    if (client == null || client.State != WebSocketState.Open)
                    {
                        Console.WriteLine("Client is null or connection is not open.");
                    }
                    else
                    {
                        await client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg)),
                            WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to client: {ex.Message}");
            }
        }


        public static void AddToRoom(WebSocket client, int roomNumber)
        {
            if (rooms.ContainsKey(roomNumber))
            {
                Room room = rooms[roomNumber];
                room.Members ??= new List<WebSocket>();

                rooms[roomNumber].Members.Add(client);

                _ = UpdateUsersMessagesAsync(client, room);
            }
            else
            {
                Room newRoom = new()
                {
                    RoomName = "",
                    RoomPassword = "",
                    RoomSecret = "",
                    Members = new List<WebSocket>()
                    {
                        client
                    },
                    Messages = new List<string>()
                };

                rooms.Add(roomNumber, newRoom);
            }
        }

        public static Room GetRoom(int roomNumber)
        {
            return rooms[roomNumber];
        }

        public static void PostToRoom(int roomNumber, HttpMessage message)
        {
            _ = UpdateRoomAsync(roomNumber, message.Username, message.Message);
        }

        private static async Task UpdateRoomAsync(int roomNumber, string user, string message)
        {
            Room room = rooms[roomNumber];
            string jsonMessage = "{\"username\":\"" + user + "\", \"message\":\"" + message + "\"}";

            room.Messages ??= new List<string>();

            room.Messages.Add(jsonMessage);

            foreach (WebSocket client in room.Members)
            {
                if (client.State == WebSocketState.Open)
                {
                    try
                    {
                        await client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonMessage)),
                            WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error broadcasting message to client: {ex.Message}");
                    }
                } else
                {
                    RemoveFromRoom(client, 1);
                }
            }
        }
    }

}