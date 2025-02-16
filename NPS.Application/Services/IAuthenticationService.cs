using Microsoft.Extensions.Configuration;

namespace NPS.Application.Services;

public interface IAuthenticationService
{
    string GenerateToken(IConfiguration configuration);
    AuthenticationService SetUsername(string userUsername);
}
