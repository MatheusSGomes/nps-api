using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace NPS.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private string _username = String.Empty;

    public AuthenticationService SetUsername(string username)
    {
        _username = username;
        return this;
    }

    public string GenerateToken(IConfiguration configuration)
    {
        double.TryParse(configuration["Authentication:Expires"], out double expires);

        var token = new JwtTokenBuilder()
            .AddUsername(_username)
            .AddSecret(configuration["Authentication:SecretKey"])
            .AddIssuer(configuration["Authentication:Issuer"])
            .AddAudience(configuration["Authentication:Audience"])
            .AddExpires(expires)
            .Build();

        return token;
    }
}
