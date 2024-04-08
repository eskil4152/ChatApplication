using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace ChatApplicationServerHttp
{
    public class RoomService
    {
        private readonly ConcurrentDictionary<Guid, List<WebSocket>> activeUsers = new();

        private readonly DatabaseService databaseService;
        private readonly ActiveUsersService activeUsersService;

        public RoomService(DatabaseService databaseService, ActiveUsersService activeUsersService)
        {
            this.databaseService = databaseService;
            this.activeUsersService = activeUsersService;
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

        public Room? EnterRoom(Room room, WebSocket socket)
        {
            if (activeUsers.ContainsKey(room.Id))
            {
                Console.WriteLine("Added user to existing room");
                activeUsers[room.Id].Add(socket);

                activeUsersService.AddActiveUser(room.Id, socket);
            } else
            {
                Console.WriteLine("Added user to new room");
                activeUsers.TryAdd(room.Id, new List<WebSocket> { socket });

                activeUsersService.AddActiveUser(room.Id, socket);
            }

            foreach (WebSocket s in activeUsers[room.Id])
            {
                Console.WriteLine("Fount user");
            }

            _ = GetAllExistingMessages(socket, room);
            
            return room;
        }

        public void PostToRoom(ChatMessage chatMessage)
        {
            Room? room = databaseService.GetRoomByName(chatMessage.RoomName);
            if (room == null) 
            {
                return;
            }

            string message = chatMessage.Username + ": " + chatMessage.Message;

            databaseService.AddMessageToRoom(room, message);

            _ = UpdateUsersInRoom(room, message);
        }

        private async Task UpdateUsersInRoom(Room room, string message)
        {
            if (activeUsers.ContainsKey(room.Id))
            {
                foreach (WebSocket client in activeUsersService.GetActiveUsers(room.Id))
                {
                    Console.WriteLine("Found client");
                    if (client.State == WebSocketState.Open)
                    {
                        try
                        {
                            List<string> list = new() { message };

                            await client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(list))),
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
            List<string> messages = new();

            try
            {
                foreach (string msg in room.Messages)
                {
                    if (client is not { State: WebSocketState.Open })
                    {
                        Console.WriteLine("Client is null or connection is not open.");
                    }
                    else
                    {
                        messages.Add(msg);
                    }
                }

                await client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(messages))),
                            WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to client: {ex.Message}");
            }
        }
    }
}