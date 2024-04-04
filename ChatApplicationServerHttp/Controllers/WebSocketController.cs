using System;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApplicationServerHttp.Controllers;

[ApiController]
public class WebSocketController : Controller
{
	private readonly WebSocketService webSocketService;

	public WebSocketController(WebSocketService webSocketService)
	{
		this.webSocketService = webSocketService;
	}

	[Route("ws")]
	public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
		{
			WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
			await webSocketService.HandleWebSocketConnection(HttpContext, webSocket);
		}
		else
		{
			Console.WriteLine("Request was not a WebSocket request");
		}
	}
}

