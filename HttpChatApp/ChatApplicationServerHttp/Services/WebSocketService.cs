using System;
using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;
using System.Text;

namespace ChatApplicationServerHttp
{
	public class WebSocketService
	{
        private readonly RoomActions roomActions;

        public WebSocketService(RoomActions roomActions)
        {
            this.roomActions = roomActions;
        }

        public async Task HandleWebSocketConnection(HttpContext context, WebSocket webSocket)
        {
            string? roomQuery = context.Request.Query["room"];
            if (string.IsNullOrEmpty(roomQuery)) return;

            Room? room = roomActions.GetRoomByName(roomQuery);

            IRequestCookieCollection cookies = context.Request.Cookies;
            if (!cookies.TryGetValue("username", out string? username) || room == null) return;

            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    ArraySegment<byte> buffer = new(new byte[1024]);
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);

                        ChatMessage chatMessage = new()
                        {
                            Message = message,
                            Username = username,
                            RoomName = room.RoomName,
                        };

                        roomActions.PostToRoom(chatMessage);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        roomActions.RemoveUserFromRoom(room, webSocket);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
            Console.WriteLine("ws conn");
        }
    }
}
}

