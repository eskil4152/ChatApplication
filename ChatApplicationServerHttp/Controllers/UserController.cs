using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ChatApplicationServerHttp.Controllers;

[ApiController]
[Route("api")]
public class LoginController : ControllerBase
{
    private readonly UserService userService;
    private readonly RoomService roomService;

    public LoginController(UserService userService, RoomService roomService)
    {
        this.userService = userService;
        this.roomService = roomService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginMessage? loginMessage)
    {
        if (loginMessage == null)
        {
            return BadRequest();
        }

        User? user = userService.Login(loginMessage);

        if (user == null)
        {
            return Unauthorized();
        }

        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.Now.AddDays(1),
        };

        Response.Cookies.Append("username", Security.Encrypt(user.Username, "key"), cookieOptions);

        return Ok(roomService.GetAllRoomsFromUser(user.Username));
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] LoginMessage? loginMessage)
    {
        if (loginMessage == null)
        {
            return BadRequest();
        }

        User? user = userService.Register(loginMessage);

        if (user == null)
        {
            return Conflict();
        }

        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.Now.AddDays(1),
        };

        Response.Cookies.Append("username", Security.Encrypt(user.Username, "key"), cookieOptions);

        return Ok(roomService.GetAllRoomsFromUser(user.Username));
    }

    [HttpGet("authenticate")]
    public IActionResult Authenticate()
    {
        IRequestCookieCollection cookies = Request.Cookies;

        if (!cookies.TryGetValue("username", out string? username)) return Unauthorized();

        if (userService.GetUser(username) == null) return Unauthorized();

        return Ok();
    }
}

