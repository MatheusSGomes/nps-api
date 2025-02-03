using Microsoft.AspNetCore.Mvc.Testing;

namespace NPS.Test.Acceptance;

public class NpsAcceptanceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public NpsAcceptanceTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _client.BaseAddress = new Uri("http://localhost:5115");
    }

    [Theory]
    [InlineData("/swagger")]
    [InlineData("/swagger/index.html")]
    public async Task Get_SwaggerIndex_ShouldReturnSuccessAndCorrectContentType(string url)
    {
        // Arrange & Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
    }

    // [Fact]
    // public async Task SubmitNps_ReturnsSuccess()
    // {
    //     // Arrange
    //     var content = new StringContent(JsonConvert.SerializeObject(new { UserId = 1, Score = 9 }), Encoding.UTF8, "application/json");
    //
    //     // Act
    //     var response = await _client.PostAsync("/api/nps/submit", content); // Supondo que existe um endpoint POST em "/api/nps/submit"
    //
    //     // Assert
    //     response.EnsureSuccessStatusCode(); // Verifica se a resposta foi 2xx
    //     var responseBody = await response.Content.ReadAsStringAsync();
    //     Assert.Contains("Success", responseBody); // Verifica se a resposta contém "Success" (exemplo de resposta)
    // }
}
