using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ChatApplicationServerHttp.Controllers;

[ApiController]
[Route("api")]
public class LoginController : ControllerBase
{
    private readonly DatabaseService databaseService;

    public LoginController(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginMessage? loginMessage)
    {
        Console.WriteLine("here123");
        if (loginMessage == null)
        {
            return BadRequest();
        }

        User? user = UserActions.Login(databaseService, loginMessage);

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
        return Ok(databaseService.GetRooms(user.Username));
    }

    [HttpPost("/register")]
    public IActionResult Register([FromBody] LoginMessage? loginMessage)
    {
        if (loginMessage == null)
        {
            return BadRequest();
        }

        User? user = UserActions.Register(databaseService, loginMessage);

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
        return Ok(databaseService.GetRooms(user.Username));
    }
}

