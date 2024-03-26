using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
namespace ChatApplicationServerHttp
{
	public class WebSocketMiddleware
	{
        private readonly DatabaseService databaseService;

        public WebSocketMiddleware(DatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        private readonly RequestDelegate requestDelegate;

		public WebSocketMiddleware(RequestDelegate requestDelegate)
		{
			this.requestDelegate = requestDelegate;
		}

		public async Task Invoke(HttpContext context)
		{
			if (context.WebSockets.IsWebSocketRequest)
			{
				WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await HandleWebSocketConnection(context, webSocket);
            } else
			{
				await requestDelegate(context);
			}
		}

		private async Task HandleWebSocketConnection(HttpContext context, WebSocket webSocket) {
            string? roomQuery = context.Request.Query["room"];
            if (string.IsNullOrEmpty(roomQuery)) return;

            Room? room = databaseService.GetRoomByName(roomQuery);

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

                        RoomActions.PostToRoom(databaseService, chatMessage);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        RoomActions.RemoveUserFromRoom(room, webSocket);

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

    public static class WebSocketMiddlewareExtensions
    {
        public static IApplicationBuilder UseWebSocketMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebSocketMiddleware>();
        }
    }
}

