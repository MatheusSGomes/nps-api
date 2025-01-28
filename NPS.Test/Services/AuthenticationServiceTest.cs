using NPS.Application.Services;

namespace NPS.Test.Services;

public class AuthenticationServiceTest
{

    [Fact]
    public void AuthenticationService_DeveImplementarUmaInterface()
    {
        // Arrange
        var authenticationService = new AuthenticationService();

        // Act & Assert
        Assert.IsAssignableFrom<IAuthenticationService>(authenticationService);
    }

    [Fact]
    public void GenerateJwtToken_DeveLancarExcecao_QuandoSecretSettingsForNula()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>()
        {
            { "Secret", null },
            { "Expires", "Authentication:Expires" },
            { "Issuer", "Authentication:Issuer" },
            { "Audience", "Authentication:Audience" }
        };

        var authenticationService = new AuthenticationService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            authenticationService.GenerateJwtToken("user", inMemorySettings));
    }
}
