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
    }

    public static class RoomActions
    {
        private static readonly Dictionary<int, Room> rooms = new();

        public static void RemoveFromRoom(WebSocket client, int roomNumber)
        {
            Room room = rooms[roomNumber];
            room.Members.Remove(client);
        }

        public static void AddToRoom(WebSocket client, int roomNumber)
        {
            if (rooms.ContainsKey(roomNumber))
            {
                Room room = rooms[roomNumber];
                room.Members ??= new List<WebSocket>();

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
                newRoom.Members = new List<WebSocket>
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

        public static void PostToRoom(int roomNumber, HttpMessage message)
        {
            Console.WriteLine("POSTED");
            _ = UpdateRoomAsync(roomNumber, message.Username, message.Message);
        }

        private static async Task UpdateRoomAsync(int roomNumber, string user, string message)
        {
            Room room = rooms[roomNumber];
            Console.WriteLine("Got room");

            foreach (WebSocket client in room.Members)
            {
                try
                {
                    string jsonMessage = "{\"username\":\"" + user + "\", \"message\":\"" + message + "\"}";
                    await client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonMessage)),
                        WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error broadcasting message to client: {ex.Message}");
                }
            }
        }
    }

}