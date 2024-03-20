﻿using System;
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
                room.MembersActive.Remove(client);
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


        public static void AddToRoom(WebSocket client, int roomNumber)
        {
            if (rooms.ContainsKey(roomNumber))
            {
                Room room = rooms[roomNumber];
                room.MembersActive ??= new List<WebSocket>();

                rooms[roomNumber].MembersActive.Add(client);


                _ = UpdateUsersMessagesAsync(client, room);
            }
            else
            {
                Room newRoom = new()
                {
                    RoomName = "",
                    RoomPassword = "",
                    RoomSecret = "",
                    MembersActive = new List<WebSocket>()
                    {
                        client
                    },
                    Members = new List<User>()
                    {
                        //TODO: Adds user to room
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

        public static void PostToRoom(int roomNumber, ChatMessage message)
        {
            //TODO: Check if user actually exists in room
            _ = UpdateRoomAsync(roomNumber, message.Username, message.Message);
        }

        private static async Task UpdateRoomAsync(int roomNumber, string user, string message)
        {
            Room room = rooms[roomNumber];
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
            }
        }
    }
}