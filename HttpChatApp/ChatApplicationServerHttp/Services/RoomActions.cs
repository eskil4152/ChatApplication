using System;
using System.Net.WebSockets;
using System.Text;

namespace ChatApplicationServerHttp
{
    public class RoomActions
    {
        private Dictionary<Guid, List<WebSocket>> activeUsers = new();
        private readonly DatabaseService databaseService;

        public RoomActions(DatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public bool JoinRoom(RoomMessage roomMessage, User user)
        {
            return databaseService.JoinRoom(roomMessage, user);
        }

        public bool CreateRoom(RoomMessage roomMessage, User user)
        {
            return databaseService.CreateRoom(roomMessage, user);
        }

        public List<Room> GetAllRoomsFromUser(string username)
        {
            return databaseService.GetRooms(username);
        }

        public Room? GetRoomByName(string roomName)
        {
            return databaseService.GetRoomByName(roomName);
        }

        public Room? EnterRoom(RoomMessage roomMessage, WebSocket socket)
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

        public void PostToRoom(ChatMessage chatMessage)
        {
            Room? room = databaseService.GetRoomByName(chatMessage.RoomName);
            if (room == null) return;

            string message = chatMessage.Username + ": " + chatMessage.Message;

            databaseService.AddMessageToRoom(room, message);

            _ = UpdateUsersInRoom(room, message);
        }

        private async Task UpdateUsersInRoom(Room room, string message)
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
        }

        public void RemoveUserFromRoom(Room room, WebSocket socket)
        {
            try
            {
                activeUsers[room.Id].Remove(socket);
            } catch (Exception e)
            {
                Console.WriteLine("Error removing socket from room: " + e.Message);
            }
        }

        public async Task GetAllExistingMessages(WebSocket client, Room room)
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