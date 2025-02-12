using Microsoft.Extensions.Configuration;

namespace NPS.Application.Services;

public interface IAuthenticationService
{
    string GenerateToken(IConfiguration configuration);
    string GenerateJwtToken(string username, Dictionary<string, string> settings);
    AuthenticationService SetUsername(string userUsername);
}
