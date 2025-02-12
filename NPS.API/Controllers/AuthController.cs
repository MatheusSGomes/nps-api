using Microsoft.AspNetCore.Mvc;
using NPS.Application.Services;

namespace NPS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthenticationService authenticationService, IConfiguration configuration)
    {
        _authenticationService = authenticationService;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLogin user)
    {
        if (user.Username == "admin" && user.Password == "password")
        {
            var token = _authenticationService
                .SetUsername(user.Username)
                .GenerateToken(_configuration);

            return Ok(new { token });
        }

        return Unauthorized();
    }
}

public record UserLogin(string Username, string Password);
