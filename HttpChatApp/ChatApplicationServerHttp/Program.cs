using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

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

        static async Task ProcessWebSocketRequest(DatabaseService databaseService, HttpListenerContext context)
        {
            HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
            WebSocket webSocket = webSocketContext.WebSocket;

            string ip = context.Request.RemoteEndPoint.Address.ToString();
            Console.WriteLine("Connected {0}", ip);

            try
            {
                while (webSocket.State == WebSocketState.Open)
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
                                    Console.WriteLine("Chat received");
                                    ChatMessage? chatMessage = JsonSerializer.Deserialize<ChatMessage>(message);

                                    if (chatMessage != null)
                                    {
                                        RoomActions.PostToRoom(1, chatMessage);
                                        Console.WriteLine("Post chat ok");
                                    }
                                    break;

                                case "LOGIN":
                                    LoginMessage? loginMessage = JsonSerializer.Deserialize<LoginMessage>(message);
                                    Console.WriteLine(loginMessage);

                                    if (loginMessage != null)
                                    {
                                        List<Room>? rooms = UserActions.CreateUser(databaseService, loginMessage);
                                        if (rooms != null)
                                        {
                                            Console.WriteLine("Able to log in");

                                            var json = new
                                            {
                                                StatusCode = 200,
                                                Rooms = rooms,
                                            };

                                            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(json))),
                                                            WebSocketMessageType.Text, true, CancellationToken.None);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Unable to log in");

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
                                    Console.WriteLine("Type join room");
                                    break;

                                case "KEY":
                                    Console.WriteLine("Type key");
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                RoomActions.RemoveFromRoom(webSocket, 1);
            }
            finally
            {
                webSocket?.Dispose();
            }
        }
    }
}