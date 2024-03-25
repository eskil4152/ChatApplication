using System;
using System.Net.WebSockets;
using System.Text;

namespace ChatApplicationServerHttp
{
    public static class RoomActions
    {
        private static readonly Dictionary<int, Room> rooms = new();

        public static bool JoinRoom(DatabaseService databaseService, RoomMessage roomMessage, User user)
        {
            return databaseService.JoinRoom(roomMessage, user);
        }

        public static bool CreateRoom(DatabaseService databaseService, RoomMessage roomMessage, User user)
        {
            /*

            Room newRoom = new()
            {
                RoomName = roomMessage.RoomName,
                RoomPassword = roomMessage.RoomPassword,
                
                Members = new List<User>()
                    {
                        userData.user
                    },
                Messages = new List<string>()
            };

            if (databaseService.CreateRoom(newRoom))
            {
                return true;
            }*/

            return false;
        }

        public static List<Room> GetAllRoomsFromUser(DatabaseService databaseService, string username)
        {
            return databaseService.GetRooms(username);
        }

        public static void PostToRoom(int roomNumber, ChatMessage message)
        {
            //TODO: Check if user actually exists in room
            _ = UpdateRoomAsync(roomNumber, message.Username, message.Message);
        }

        private static async Task UpdateRoomAsync(int roomNumber, string user, string message)
        {
            /*Room room = rooms[roomNumber];
            string jsonMessage = "{\"username\":\"" + user + "\", \"message\":\"" + message + "\"}";

            room.Messages ??= new List<string>();

            room.Messages.Add(jsonMessage);

            foreach (WebSocket client in room.MembersActive)
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
                }
                else
                {
                    RemoveFromRoom(client, 1);
                }
            }*/
        }

        public static async Task GetAllExistingMessages(WebSocket client, Room room)
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
    }
}