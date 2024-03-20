using System;
using System.Net.WebSockets;
using System.Text;

namespace ChatApplicationServerHttp
{
    public static class RoomActions
    {
        private static readonly Dictionary<int, Room> rooms = new();

        public static void RemoveFromRoom(WebSocket client, int roomNumber)
        {
            try
            {
                Room room = rooms[roomNumber];
                //room.MembersActive.Remove(client);
            } catch
            {
                Console.WriteLine("");
            }
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


        public static bool AddToRoom(DatabaseService databaseService, RoomMessage roomMessage)
        {
            if (roomMessage.RoomType == RoomType.JOIN)
            {
                if (databaseService.JoinRoom(roomMessage))
                {
                    Console.WriteLine("Joined");

                    return true;
                } else
                {
                    Console.WriteLine("Unable to join");
                }
            }
            else if (roomMessage.RoomType == RoomType.CREATE)
            {
                Room newRoom = new()
                {
                    RoomName = roomMessage.RoomName,
                    RoomPassword = roomMessage.RoomPassword,
                    /*MembersActive = new List<WebSocket>()
                    {
                        client
                    },*/
                    Members = new List<string>()
                    {
                        roomMessage.Username
                    },
                    Messages = new List<string>()
                };

                if (databaseService.CreateRoom(newRoom))
                {
                    return true;
                }
            }

            return false;
        }

        public static Room GetRoom(int roomNumber)
        {
            return rooms[roomNumber];
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
    }
}