using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace ChatApplicationServerHttp
{
	public class WebSocketRequest
	{
		public static async Task ProcessWebSocketRequest(HttpListenerContext context, DatabaseService databaseService)
		{
            HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
            WebSocket webSocket = webSocketContext.WebSocket;
            
            Console.WriteLine("Connected {0}", context.Request.RemoteEndPoint.Address.ToString());

            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    ArraySegment<byte> buffer = new(new byte[1024]);
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                        ChatMessage? chatMessage = JsonSerializer.Deserialize<ChatMessage>(message);

                        RoomActions.PostToRoom(1, chatMessage);

                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        //RoomActions.RemoveFromRoom(webSocket, 1);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

        }
	}
}

