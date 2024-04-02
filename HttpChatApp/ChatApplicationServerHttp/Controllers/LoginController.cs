using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ChatApplicationServerHttp.Controllers;

[ApiController]
[Route("api")]
public class LoginController : ControllerBase
{
    private readonly UserActions userActions;
    private readonly RoomActions roomActions;

    public LoginController(UserActions userActions, RoomActions roomActions)
    {
        this.userActions = userActions;
        this.roomActions = roomActions;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginMessage? loginMessage)
    {
        if (loginMessage == null)
        {
            return BadRequest();
        }

        User? user = userActions.Login(loginMessage);

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

        return Ok(roomActions.GetAllRoomsFromUser(user.Username));
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] LoginMessage? loginMessage)
    {
        if (loginMessage == null)
        {
            return BadRequest();
        }

        User? user = userActions.Register(loginMessage);

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

        return Ok(roomActions.GetAllRoomsFromUser(user.Username));
    }
}

