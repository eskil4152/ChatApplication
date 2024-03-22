using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Collections.Concurrent;

namespace ChatApplicationServerHttp
{
    class Program
    {
        static async Task Main()
        {
            var dbContext = new DatabaseContext();
            var databaseService = new DatabaseService(dbContext);

            HttpListener listener = new();
            listener.Prefixes.Add("http://192.168.0.135:8083/");
            listener.Prefixes.Add("http://localhost:8083/");
            listener.Start();
            Console.WriteLine("Listening for HTTP connections...");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();

                if (context.Request.IsWebSocketRequest)
                {
                    await WebSocketRequest.ProcessWebSocketRequest(context, databaseService);
                } else
                {
                    if (context.Request.HttpMethod == "OPTIONS")
                    {
                        await WriteResponse.WriteOptionsResponse(context);
                    } else
                    {
                        await HttpRequests.ProcessRestRequest(context, databaseService);
                    }
                }
            }
        }
        
        /*static async Task Main()
        {
            var dbContext = new DatabaseContext();
            var databaseService = new DatabaseService(dbContext);

            HttpListener listener = new();
            listener.Prefixes.Add("http://192.168.0.135:8083/");
            listener.Start();
            Console.WriteLine("Listening for WebSocket connections...");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    await ProcessWebSocketRequest(databaseService, context);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private static ConcurrentDictionary<string, UserData> sessionData = new();

        static async Task ProcessWebSocketRequest(DatabaseService databaseService, HttpListenerContext context)
        {
            HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
            WebSocket webSocket = webSocketContext.WebSocket;

            string sessionId = context?.Request?.Cookies["sessionId"]?.Value ?? Guid.NewGuid().ToString();
            UserData userData;

            if (sessionData.TryGetValue(sessionId, out userData))
            {
                Console.WriteLine($"User {userData.user.Username} reconnected.");
                userData.WebSocket = webSocket; // Update WebSocket reference
            }
            else
            {
                Console.WriteLine("New user connected.");
                userData = new UserData { WebSocket = webSocket };
                sessionData.TryAdd(sessionId, userData);
            }

            Cookie cookie = new("sessionId", sessionId);
            Console.WriteLine("Cookie: " + cookie);

            context.Response.SetCookie(new Cookie("sessionId", sessionId));
            context.Response.Headers.Add("Set-Cookie", $"sessionId={sessionId}; Path=/; HttpOnly; SameSite=Strict");

            string ip = context.Request.RemoteEndPoint.Address.ToString();
            Console.WriteLine("Connected {0}", ip);

            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    ArraySegment<byte> buffer = new(new byte[1024]);
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);

                        JsonElement parsedMessage = JsonDocument.Parse(message).RootElement;
                        if (parsedMessage.TryGetProperty("Type", out JsonElement jsonElement))
                        {
                            switch (jsonElement.ToString())
                            {
                                case "CHAT":
                                    ChatMessage? chatMessage = JsonSerializer.Deserialize<ChatMessage>(message);

                                    if (chatMessage != null)
                                    {
                                        RoomActions.PostToRoom(1, chatMessage);
                                    }
                                    break;

                                case "LOGIN":
                                    if (userData.user != null)
                                    {
                                        Console.WriteLine("Already logged in");
                                        break;
                                    }

                                    LoginMessage? loginMessage = JsonSerializer.Deserialize<LoginMessage>(message);
                                    Console.WriteLine(loginMessage);

                                    if (loginMessage != null)
                                    {
                                        User? user = UserActions.LoginRegister(databaseService, loginMessage);
                                        if (user != null)
                                        {
                                            userData.user = user;
                                            var json = new
                                            {
                                                StatusCode = 200,
                                                Rooms = user.Rooms,
                                            };

                                            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(json))),
                                                            WebSocketMessageType.Text, true, CancellationToken.None);
                                        }
                                        else
                                        {
                                            var json = new
                                            {
                                                StatusCode = 401,
                                                Rooms = "",
                                            };

                                            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(json))),
                                                            WebSocketMessageType.Text, true, CancellationToken.None);
                                        }
                                    }
                                    break;

                                case "JOINROOM":
                                    if (userData.user != null)
                                    {
                                        Console.WriteLine("Not logged in");
                                        break;
                                    }

                                    RoomMessage? roomMessage = JsonSerializer.Deserialize<RoomMessage>(message);
                                    if (roomMessage != null)
                                    {
                                        var json = new
                                        {
                                            StatusCode = RoomActions.AddToRoom(databaseService, roomMessage, userData) ? 200 : 401,
                                        };

                                        await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(json))),
                                                        WebSocketMessageType.Text, true, CancellationToken.None);
                                    }
                                    else
                                    {
                                        var json = new
                                        {
                                            StatusCode = 401,
                                        };

                                        await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(json))),
                                                        WebSocketMessageType.Text, true, CancellationToken.None);
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        RoomActions.RemoveFromRoom(webSocket, 1);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            RoomActions.RemoveFromRoom(webSocket, 1);
            webSocket?.Dispose();
            CleanupDisconnectedSessions();
        }

        static void CleanupDisconnectedSessions()
        {
            foreach (var kvp in sessionData)
            {
                if (kvp.Value.WebSocket.State != WebSocketState.Open)
                {
                    sessionData.TryRemove(kvp.Key, out _);
                }
            }
        }*/
    }
}