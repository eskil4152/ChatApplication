using System;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace ChatApplicationServerHttp.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController : Controller
{
    private readonly DatabaseService databaseService;

    public RoomsController(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
    }

    [HttpGet("all")]
    public IActionResult GetAllRooms()
    {
        IRequestCookieCollection cookies = Request.Cookies;
        Console.WriteLine("Cookies" + cookies["username"]);
        
        if (!cookies.TryGetValue("username", out string? usernameCookie)) return Unauthorized();

        string decryptedUsername = Security.Decrypt(usernameCookie, "key");

        List<Room> rooms = RoomActions.GetAllRoomsFromUser(databaseService, decryptedUsername);
        return Ok(rooms);
    }

    [HttpPost("join")]
    public IActionResult JoinRoom([FromBody] RoomMessage roomMessage)
    {
        IRequestCookieCollection cookies = Request.Cookies;
        if (!cookies.TryGetValue("username", out string? usernameCookie)) return Unauthorized();

        User? user = databaseService.GetUser(Security.Decrypt(usernameCookie, "key"));
        if (user == null) return Unauthorized();

        databaseService.JoinRoom(roomMessage, user);

        return Ok();
    }

    [HttpPost("create")]
    public IActionResult CreateRoom([FromBody] RoomMessage roomMessage)
    {
        IRequestCookieCollection cookies = Request.Cookies;
        if (!cookies.TryGetValue("username", out string? usernameCookie)) return Unauthorized();

        User? user = databaseService.GetUser(Security.Decrypt(usernameCookie, "key"));
        if (user == null) return Unauthorized();

        databaseService.CreateRoom(roomMessage, user);

        return Ok();
    }

    [HttpPost("enter")]
    public IActionResult EnterRoom([FromQuery] string roomName)
    {
        IRequestCookieCollection cookies = Request.Cookies;
        if (!cookies.TryGetValue("username", out string? usernameCookie)) return Unauthorized();

        User? user = databaseService.GetUser(Security.Decrypt(usernameCookie, "key"));
        if (user == null) return Unauthorized();

        Room? room = databaseService.GetRoomByName(roomName);
        if (room == null || !room.Members.Contains(user.Id)) return NotFound();

        string roomIdentifier = Security.Encrypt(room.Id.ToString(), "key");

        return Ok(roomIdentifier);
    }
}