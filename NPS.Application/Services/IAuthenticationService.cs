namespace NPS.Application.Services;

public interface IAuthenticationService
{
    string GenerateJwtToken(string username, Dictionary<string, string> configs);
}
