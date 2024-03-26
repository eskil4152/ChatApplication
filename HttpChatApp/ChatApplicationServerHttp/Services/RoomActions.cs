using System;
using System.Net.WebSockets;
using System.Text;

namespace ChatApplicationServerHttp
{
    public static class RoomActions
    {
        private static Dictionary<Guid, List<WebSocket>> activeUsers = new();

        public static bool JoinRoom(DatabaseService databaseService, RoomMessage roomMessage, User user)
        {
            return databaseService.JoinRoom(roomMessage, user);
        }

        public static bool CreateRoom(DatabaseService databaseService, RoomMessage roomMessage, User user)
        {
            return databaseService.CreateRoom(roomMessage, user);
        }

        public static List<Room> GetAllRoomsFromUser(DatabaseService databaseService, string username)
        {
            return databaseService.GetRooms(username);
        }

        public static Room? EnterRoom(DatabaseService databaseService, RoomMessage roomMessage, User user, WebSocket socket)
        {
            //TODO: Check if user actually exists in room

            Room? room = databaseService.GetRoomByName(roomMessage.RoomName);
            if (room == null) return null;

            if (activeUsers.ContainsKey(room.Id))
            {
                activeUsers[room.Id].Add(socket);
            } else
            {
                activeUsers.Add(room.Id, new List<WebSocket>() { socket });
            }

            return room;
        }

        public static void PostToRoom(DatabaseService databaseService, RoomMessage roomMessage, User user)
        {
            //TODO: Check if user actually exists in room

            Room? room = databaseService.GetRoomByName(roomMessage.RoomName);
            if (room == null) return;

            room.Messages.Add(user.Username + ": " + "Test");

            _ = UpdateUsersInRoom(room, "Hello");
        }

        private static async Task UpdateUsersInRoom(Room room, string message)
        {
            if (activeUsers.ContainsKey(room.Id))
            {
                foreach (WebSocket client in activeUsers[room.Id])
                {
                    if (client.State == WebSocketState.Open)
                    {
                        try
                        {
                            await client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)),
                                WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error broadcasting message to client: {ex.Message}");
                        }
                    } else
                    {
                        activeUsers[room.Id].Remove(client);
                    }
                }
            } else
            {
                Console.WriteLine("Error, room should exist in dict");
            }
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

        private static async Task RemoveUserFromRoom(Room room, WebSocket socket)
        {
            try
            {
                activeUsers[room.Id].Remove(socket);
            } catch (Exception e)
            {
                Console.WriteLine("Error removing socket from room: " + e.Message);
            }
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