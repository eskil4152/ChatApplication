using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace ChatApplicationServerHttp
{
    class Program
    {
        public static async Task Main()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8083/");
            listener.Start();
            Console.WriteLine("Listening for WebSocket connections...");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    ProcessWebSocketRequest(context);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        static async Task ProcessWebSocketRequest(HttpListenerContext context)
        {
            HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
            WebSocket webSocket = webSocketContext.WebSocket;

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    RoomActions.AddToRoom(webSocket, 1);

                    ArraySegment<byte> buffer = new(new byte[1024]);
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                        var deserializedMessage = JsonSerializer.Deserialize<HttpMessage>(message);

                        switch (deserializedMessage.Type)
                        {
                            case MessageType.CHAT:
                                Console.WriteLine("Chat received");
                                RoomActions.PostToRoom(1, deserializedMessage);
                                break;
                            case MessageType.LOGIN:
                                Console.WriteLine("Type login");
                                break;
                            case MessageType.ROOMSELECT:
                                Console.WriteLine("Type room select");
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