using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace ChatApplicationServerHttp
{
    class Program
    {
        private readonly DatabaseService databaseService;

        public Program(DatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        // Return all of the users saved rooms
        public void CreateUser(LoginMessage loginMessage, WebSocket webSocket)
        {
            if (loginMessage.LoginType == LoginType.LOGIN)
            {
                User? user = databaseService.CheckUser(loginMessage.Username, loginMessage.Password);

                if (user == null)
                {
                    Console.WriteLine("Return rooms");
                } else
                {
                    Console.WriteLine("");
                }
            } else if (loginMessage.LoginType == LoginType.REGISTER)
            {
                User user = new()
                {
                    Username = loginMessage.Username,
                    Password = Password.HashPassword(loginMessage.Password),
                    Rooms = new List<Room>(),
                };

                databaseService.Register(user);
            } else
            {
                Console.WriteLine("Invalid login type: " + loginMessage.LoginType);
            }
        }

        public async Task Main()
        {
            HttpListener listener = new();
            listener.Prefixes.Add("http://192.168.0.135:8083/");
            listener.Start();
            Console.WriteLine("Listening for WebSocket connections...");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    await ProcessWebSocketRequest(context);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        async Task ProcessWebSocketRequest(HttpListenerContext context)
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
                        var deserializedMessage = JsonSerializer.Deserialize<HttpMessage>(message);

                        switch (deserializedMessage?.Type)
                        {
                            case MessageType.CHAT:
                                Console.WriteLine("Chat received");
                                ChatMessage? chatMessage = JsonSerializer.Deserialize<ChatMessage>(message);

                                if (chatMessage != null)
                                {
                                    RoomActions.PostToRoom(1, chatMessage);
                                    Console.WriteLine("Post chat ok");
                                }
                                break;
                            case MessageType.LOGIN:
                                LoginMessage? loginMessage = JsonSerializer.Deserialize<LoginMessage>(message);

                                if (loginMessage != null)
                                {
                                    CreateUser(loginMessage, webSocket);
                                    Console.WriteLine("Login ok");
                                }
                                break;
                            case MessageType.JOINROOM:
                                Console.WriteLine("Type join room");
                                break;
                            case MessageType.KEY:
                                Console.WriteLine("Type key");
                                break;
                            default:
                                break;
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