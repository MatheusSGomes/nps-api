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
            var settings = new Dictionary<string, string>()
            {
                { "Secret", _configuration["Authentication:SecretKey"]! },
                { "Expires", _configuration["Authentication:Expires"]! },
                { "Issuer", _configuration["Authentication:Issuer"]! },
                { "Audience", _configuration["Authentication:Audience"]! }
            };

            var token = _authenticationService.GenerateJwtToken(user.Username, settings);
            return Ok(new { token });
        }

        return Unauthorized();
    }
}

public record UserLogin(string Username, string Password);
