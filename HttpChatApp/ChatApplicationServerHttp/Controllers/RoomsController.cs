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
    private readonly RoomActions roomActions;
    private readonly UserActions userActions;

    public RoomsController(RoomActions roomActions, UserActions userActions)
    {
        this.roomActions = roomActions;
        this.userActions = userActions;
    }

    [HttpGet("all")]
    public IActionResult GetAllRooms()
    {
        IRequestCookieCollection cookies = Request.Cookies;
        Console.WriteLine("Cookies" + cookies["username"]);
        
        if (!cookies.TryGetValue("username", out string? usernameCookie)) return Unauthorized();

        string decryptedUsername = Security.Decrypt(usernameCookie, "key");

        List<Room> rooms = roomActions.GetAllRoomsFromUser(decryptedUsername);
        return Ok(rooms);
    }

    [HttpPost("join")]
    public IActionResult JoinRoom([FromBody] RoomMessage roomMessage)
    {
        IRequestCookieCollection cookies = Request.Cookies;
        if (!cookies.TryGetValue("username", out string? usernameCookie)) return Unauthorized();

        User? user = userActions.GetUser(usernameCookie);
        if (user == null) return Unauthorized();

        if (roomActions.JoinRoom(roomMessage, user))
        {
            return Ok();
        }

        return NotFound();
    }

    [HttpPost("create")]
    public IActionResult CreateRoom([FromBody] RoomMessage roomMessage)
    {
        IRequestCookieCollection cookies = Request.Cookies;
        if (!cookies.TryGetValue("username", out string? usernameCookie)) return Unauthorized();

        User? user = userActions.GetUser(usernameCookie);
        if (user == null) return Unauthorized();

        if(roomActions.CreateRoom(roomMessage, user))
        {
            return Ok();
        }

        return Ok();
    }

    [HttpPost("enter")]
    public IActionResult EnterRoom([FromQuery] string roomName)
    {
        IRequestCookieCollection cookies = Request.Cookies;
        if (!cookies.TryGetValue("username", out string? usernameCookie)) return Unauthorized();

        User? user = userActions.GetUser(usernameCookie);
        if (user == null) return Unauthorized();

        Room? room = roomActions.GetRoomByName(roomName);
        if (room == null || !room.Members.Contains(user.Id)) return NotFound();

        string roomIdentifier = Security.Encrypt(room.Id.ToString(), "key");

        return Ok(roomIdentifier);
    }
}