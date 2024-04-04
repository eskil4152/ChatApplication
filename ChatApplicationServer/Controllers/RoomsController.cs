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
    private readonly RoomService roomService;
    private readonly UserService userService;

    public RoomsController(RoomService roomService, UserService userService)
    {
        this.roomService = roomService;
        this.userService = userService;
    }

    [HttpGet("all")]
    public IActionResult GetAllRooms()
    {
        IRequestCookieCollection cookies = Request.Cookies;
        Console.WriteLine("Cookies" + cookies["username"]);
        
        if (!cookies.TryGetValue("username", out string? usernameCookie)) 
        {
            return Unauthorized();
        }

        string decryptedUsername = Security.Decrypt(usernameCookie, "key");

        List<Room> rooms = roomService.GetAllRoomsFromUser(decryptedUsername);
        return Ok(rooms);
    }

    [HttpPost("join")]
    public IActionResult JoinRoom([FromBody] RoomMessage roomMessage)
    {
        IRequestCookieCollection cookies = Request.Cookies;
        if (!cookies.TryGetValue("username", out string? usernameCookie))
        {
            return Unauthorized();   
        }

        User? user = userService.GetUser(usernameCookie);
        if (user == null) 
        {
            return Unauthorized();
        }

        if (roomService.JoinRoom(roomMessage, user))
        {
            return Ok();
        }

        return NotFound();
    }

    [HttpPost("create")]
    public IActionResult CreateRoom([FromBody] RoomMessage roomMessage)
    {
        IRequestCookieCollection cookies = Request.Cookies;
        if (!cookies.TryGetValue("username", out string? usernameCookie)) 
        {
            return Unauthorized();
        }

        User? user = userService.GetUser(usernameCookie);
        if (user == null)
        {
            return Unauthorized();
        }

        if(roomService.CreateRoom(roomMessage, user))
        {
            return Ok();
        }

        return Ok();
    }

    [HttpPost("enter")]
    public IActionResult EnterRoom([FromQuery] string roomName)
    {
        IRequestCookieCollection cookies = Request.Cookies;
        if (!cookies.TryGetValue("username", out string? usernameCookie)) 
        {
            return Unauthorized();
        }

        User? user = userService.GetUser(usernameCookie);
        if (user == null) 
        {
            return Unauthorized();
        }

        Room? room = roomService.GetRoomByName(roomName);
        if (room == null || !room.Members.Contains(user.Id)) 
        {
            return NotFound();
        }

        return Ok();
    }
}