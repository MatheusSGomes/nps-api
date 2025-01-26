using Microsoft.AspNetCore.Mvc;
using NPS.Application.Services;

namespace NPS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLogin user)
    {
        // TODO: Validação de senha
        if (user.Username == "admin" && user.Password == "password")
        {
            var token = _authenticationService.GenerateJwtToken(user.Username);
            return Ok(new { token });
        }

        return Unauthorized();
    }
}

public record UserLogin(string Username, string Password);
