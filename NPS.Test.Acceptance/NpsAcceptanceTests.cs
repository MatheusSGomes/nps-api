using System.Net;
using System.Text;
using System.Web;
using Bogus;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NPS.Application;
using NPS.Application.Services;
using NPS.Core.Nps.ViewModels;

namespace NPS.Test.Acceptance;

public class NpsAcceptanceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Faker _faker;
    private readonly IConfiguration _configuration;

    public NpsAcceptanceTests(WebApplicationFactory<Program> factory)
    {
        _faker = new Faker();
        _client = factory.CreateClient();
        _client.BaseAddress = new Uri("http://localhost:5115");
        string testSettings = "appsettings.Test.json";

        var currentDirectory = Directory.GetCurrentDirectory();
        var configurationRoot = new ConfigurationBuilder()
            .SetBasePath(currentDirectory)
            .AddJsonFile(testSettings)
            .Build();

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IConfiguration>(configurationRoot)
            .BuildServiceProvider();

        _configuration = serviceProvider.GetRequiredService<IConfiguration>();
    }

    [Theory]
    [InlineData("/swagger")]
    [InlineData("/swagger/index.html")]
    public async Task Get_SwaggerIndex_DeveRetornarSucesso_QuandoContentTypeEstiverCorreto(string uri)
    {
        // Arrange & Act
        var response = await _client.GetAsync(uri);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
    }

    [Fact]
    public async Task Post_AuthLogin_DeveRetornarUnauthorized_CasoUsernameEPasswordEstejamIncorretos()
    {
        // Arrange
        string uri = "/api/Auth/login";
        string mediaType = "application/json";
        string usernameInvalido = _faker.Person.UserName;
        string passwordInvalido = _faker.Random.String();

        var serializeObject =
            JsonConvert.SerializeObject(new { username = usernameInvalido, password = passwordInvalido });
        var content = new StringContent(serializeObject, Encoding.UTF8, mediaType);

        // Act
        var response = await _client.PostAsync(uri, content);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_AuthLogin_DeveRetornarUnauthorized_CasoUsernameEstejaIncorreto()
    {
        // Arrange
        string uri = "/api/Auth/login";
        string mediaType = "application/json";
        string usernameInexistente = _faker.Person.UserName;

        var serializeObject =
            JsonConvert.SerializeObject(new { username = usernameInexistente, password = "password" });
        var content = new StringContent(serializeObject, Encoding.UTF8, mediaType);

        // Act
        var response = await _client.PostAsync(uri, content);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_AuthLogin_DeveRetornarUnauthorized_CasoPasswordEstejaIncorreto()
    {
        // Arrange
        string uri = "/api/Auth/login";
        string mediaType = "application/json";
        string wrongPassword = "WRONG_PASSWORD";

        var serializeObject = JsonConvert.SerializeObject(new { username = "admin", password = wrongPassword });
        var content = new StringContent(serializeObject, Encoding.UTF8, mediaType);

        // Act
        var response = await _client.PostAsync(uri, content);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_AuthLogin_DeveRetornarToken_QuandoCredenciaisEstejamCorretas()
    {
        // Arrange
        string uri = "/api/Auth/login";
        string mediaType = "application/json";

        var serializeObject = JsonConvert.SerializeObject(new { username = "admin", password = "password" });
        var content = new StringContent(serializeObject, Encoding.UTF8, mediaType);

        // Act
        var response = await _client.PostAsync(uri, content);

        // Assert
        response.EnsureSuccessStatusCode(); // Verifica se a resposta foi 2xx
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Contains("token", responseBody);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(-100)]
    public async Task Post_NpsResponses_DeveRetornarErro_QuandoScoreForMenorOuIgualZero(int expectedScore)
    {
        // Arrange
        string uri = "/v1/Nps/Responses";
        string mediaType = "application/json";
        var serializedObject = JsonConvert.SerializeObject(
            new
            {
                score = expectedScore, customerName = "Gerado pelo Teste de Aceitação",
                comment = "Gerado pelo Teste de Aceitação"
            });
        var content = new StringContent(serializedObject, Encoding.UTF8, mediaType);

        // Act
        var response = await _client.PostAsync(uri, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        string expectedContent = "Invalid Score";
        Assert.Contains(expectedContent, responseBody);
    }

    [Theory]
    [InlineData(11)]
    [InlineData(20)]
    [InlineData(30)]
    public async Task Post_NpsResponses_DeveRetornarUmErro_QuandoScoreForMaiorQue10(int expectedScore)
    {
        // Arrange
        string uri = "/v1/Nps/Responses";
        string mediaType = "application/json";
        var serializedObject = JsonConvert.SerializeObject(
            new
            {
                score = expectedScore, customerName = "Gerado pelo Teste de Aceitação",
                comment = "Gerado pelo Teste de Aceitação"
            });
        var content = new StringContent(serializedObject, Encoding.UTF8, mediaType);

        // Act
        var response = await _client.PostAsync(uri, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        string expectedContent = "Invalid Score";
        Assert.Contains(expectedContent, responseBody);
    }

    [Fact]
    public async Task Post_NpsResponses_DeveRetornarUmErro_QuandoCustomerNameForVazio()
    {
        // Arrange
        string uri = "/v1/Nps/Responses";
        string mediaType = "application/json";
        string emptyExpectedUsername = "";
        var serializedObject = JsonConvert.SerializeObject(
            new { score = 5, customerName = emptyExpectedUsername, comment = "Gerado pelo Teste de Aceitação" });
        var content = new StringContent(serializedObject, Encoding.UTF8, mediaType);

        // Act
        var response = await _client.PostAsync(uri, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        string expectedContent = "Customer name is null or empty";
        Assert.Contains(expectedContent, responseBody);
    }

    [Fact]
    public async Task Post_NpsResponses_DeveRetornarSucesso_QuandoTodosOsDadosEstiveremOk()
    {
        // Arrange
        string uri = "/v1/Nps/Responses";
        string mediaType = "application/json";
        var objToSerialize = new
            { score = 5, customerName = _faker.Person.UserName, comment = "Gerado pelo Teste de Aceitação" };

        var serializedObject = JsonConvert.SerializeObject(objToSerialize);
        var content = new StringContent(serializedObject, Encoding.UTF8, mediaType);

        // Act
        HttpResponseMessage response = await _client.PostAsync(uri, content);
        string responseBody = await response.Content.ReadAsStringAsync();
        var npsResponse = JsonConvert.DeserializeObject<NpsResponseViewModel>(responseBody);

        // Assert
        Assert.Equal(objToSerialize.score, npsResponse.Score);
        Assert.Equal(objToSerialize.customerName, npsResponse.CustomerName);
    }

    [Fact]
    public async Task Get_NpsResponses_DeveRetornarUnauthorized_QuandoNenhumTokenForEnviado()
    {
        // Arrange
        var uri = "/v1/Nps/Responses";

        // Act
        var response = await _client.GetAsync(uri);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_NpsResponses_DeveRetornarUnauthorized_QuandoTokenEnviadoForInvalido()
    {
        // Arrange
        string uri = "/v1/Nps/Responses";
        var username = _faker.Person.UserName;
        var inMemorySettings = new Dictionary<string, string>()
        {
            { "Authentication:SecretKey", Guid.NewGuid().ToString() },
            { "Authentication:Issuer", _faker.Random.Hash() },
            { "Authentication:Audience", _faker.Random.Hash() },
            { "Authentication:Expires", "30" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var authService = new AuthenticationService();
        var randomToken = authService.SetUsername(username).GenerateToken(configuration);

        // Act
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + randomToken);
        var response = await _client.GetAsync(uri);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_NpsResponses_DeveRetornarUmErro_QuandoQueryParamDataForInvalida()
    {
        // Arrange
        string uri = "v1/Nps/Responses";

        var tokenJwt = new AuthenticationService()
            .SetUsername(_faker.Person.UserName).GenerateToken(_configuration);

        // Act
        // Definindo a URL base
        UriBuilder uriBuilder = new UriBuilder(_client.BaseAddress)
        {
            Path = "/v1/Nps/Responses"
        };

        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["Data"] = "VALOR_INVÁLIDO";

        // Atualizando a URL com os parâmetros de consulta
        uriBuilder.Query = query.ToString();

        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenJwt);
        var responseClient = await _client.GetAsync(uriBuilder.Uri.PathAndQuery);

        string responseSerialized = await responseClient.Content.ReadAsStringAsync();
        var responseDeserialized = JsonConvert.DeserializeObject<ErrorResponse>(responseSerialized);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, responseClient.StatusCode);
        Assert.Contains("errors", responseSerialized);
        Assert.True(responseDeserialized.Errors.Count >= 1);
        Assert.Equal(responseDeserialized.Errors["Data"][0], "The value 'VALOR_INVÁLIDO' is not valid.");
    }

    [Fact]
    public async Task Get_NpsResponses_DeveRetornarSucesso_QuandoQueryParamDataForValida()
    {
        // Arrange
        string uri = "v1/Nps/Responses";

        var tokenJwt = new AuthenticationService()
            .SetUsername(_faker.Person.UserName).GenerateToken(_configuration);

        // Act
        // Definindo a URL base
        UriBuilder uriBuilder = new UriBuilder(_client.BaseAddress)
        {
            Path = "/v1/Nps/Responses"
        };

        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["Data"] = _faker.Date.PastDateOnly().ToString("yyyy-MM-dd");

        // Atualizando a URL com os parâmetros de consulta
        uriBuilder.Query = query.ToString();

        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenJwt);
        var responseClient = await _client.GetAsync(uriBuilder.Uri.PathAndQuery);

        string responseSerialized = await responseClient.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseClient.StatusCode);
        Assert.DoesNotContain(responseSerialized, "error");
        Assert.True(responseClient.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Get_NpsResponses_DeveRetornarSucesso_QuandoQueryParamCustomerNameForValido()
    {
        // Arrange
        string uri = "v1/Nps/Responses";

        var tokenJwt = new AuthenticationService()
            .SetUsername(_faker.Person.UserName).GenerateToken(_configuration);

        // Act
        // Definindo a URL base
        UriBuilder uriBuilder = new UriBuilder(_client.BaseAddress)
        {
            Path = "/v1/Nps/Responses"
        };

        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["CustomerName"] = _faker.Person.FullName;

        // Atualizando a URL com os parâmetros de consulta
        uriBuilder.Query = query.ToString();

        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenJwt);
        var responseClient = await _client.GetAsync(uriBuilder.Uri.PathAndQuery);

        string responseSerialized = await responseClient.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseClient.StatusCode);
        Assert.DoesNotContain(responseSerialized, "error");
        Assert.True(responseClient.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Get_NpsResponses_DeveRetornarUmErro_QuandoQueryParamCategoryForInvalida()
    {
        // Arrange
        string uri = "v1/Nps/Responses";

        var tokenJwt = new AuthenticationService()
            .SetUsername(_faker.Person.UserName).GenerateToken(_configuration);

        // Act
        UriBuilder uriBuilder = new UriBuilder(_client.BaseAddress)
        {
            Path = "/v1/Nps/Responses"
        };

        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["Category"] = _faker.Random.String();

        // Atualizando a URL com os parâmetros de consulta
        uriBuilder.Query = query.ToString();

        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenJwt);
        var responseClient = await _client.GetAsync(uriBuilder.Uri.PathAndQuery);

        string responseSerialized = await responseClient.Content.ReadAsStringAsync();
        var responseDeserialized = JsonConvert.DeserializeObject<ErrorResponse>(responseSerialized);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, responseClient.StatusCode);
        Assert.Contains("errors", responseSerialized);
        Assert.True(responseDeserialized.Errors.Count >= 1);
        Assert.Equal(responseDeserialized.Title, "One or more validation errors occurred.");
    }

    [Fact]
    public async Task Get_NpsResponses_DeveRetornarSucesso_QuandoQueryParamCategoryForValido()
    {
        // Arrange
        string uri = "v1/Nps/Responses";

        var tokenJwt = new AuthenticationService()
            .SetUsername(_faker.Person.UserName).GenerateToken(_configuration);

        // Act
        // Definindo a URL base
        UriBuilder uriBuilder = new UriBuilder(_client.BaseAddress)
        {
            Path = "/v1/Nps/Responses"
        };

        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        string categoryParamValido = _faker.Random.Int(0, 100).ToString();
        query["Category"] = categoryParamValido;

        // Atualizando a URL com os parâmetros de consulta
        uriBuilder.Query = query.ToString();

        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenJwt);
        var responseClient = await _client.GetAsync(uriBuilder.Uri.PathAndQuery);

        string responseSerialized = await responseClient.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseClient.StatusCode);
        Assert.DoesNotContain(responseSerialized, "error");
        Assert.True(responseClient.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Get_NpsResponses_DeveRetornarSucesso_QuandoTodosQueryParamsForemValido()
    {
        // Arrange
        string uri = "v1/Nps/Responses";

        var tokenJwt = new AuthenticationService()
            .SetUsername(_faker.Person.UserName).GenerateToken(_configuration);

        // Act
        // Definindo a URL base
        UriBuilder uriBuilder = new UriBuilder(_client.BaseAddress)
        {
            Path = "/v1/Nps/Responses"
        };

        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        string categoryParamValido = _faker.Random.Int(0, 100).ToString();
        string customerNameParamValido = _faker.Person.FullName;
        string dataParamValido = _faker.Date.PastDateOnly().ToString("yyyy-MM-dd");

        query["Category"] = categoryParamValido;
        query["CustomerName"] = customerNameParamValido;
        query["Data"] = dataParamValido;

        // Atualizando a URL com os parâmetros de consulta
        uriBuilder.Query = query.ToString();

        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenJwt);
        var responseClient = await _client.GetAsync(uriBuilder.Uri.PathAndQuery);

        string responseSerialized = await responseClient.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseClient.StatusCode);
        Assert.DoesNotContain(responseSerialized, "error");
        Assert.True(responseClient.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Get_NpsScore_DeveRetornarStatusCodeUnauthorized_QuandoONenhumTokenDeAcessoForEnviado()
    {
        // Arrange
        string uri = "v1/Nps/Score";

        // Act
        var responseClient = await _client.GetAsync(uri);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, responseClient.StatusCode);
        Assert.False(responseClient.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Get_NpsScore_DeveRetornarStatusCodeUnauthorized_QuandoOTokenDeAcessoForInvalido()
    {
        // Arrange
        string uri = "v1/Nps/Score";

        var inMemorySettings = new Dictionary<string, string>
        {
            { "Authentication:SecretKey", Guid.NewGuid().ToString() },
            { "Authentication:Issuer", "ALEATÓRIO" },
            { "Authentication:Audience", "ALEATÓRIO" },
            { "Authentication:Expires", "30" },
        };

        var wrongConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var tokenJwt = new AuthenticationService()
            .SetUsername(_faker.Person.UserName).GenerateToken(wrongConfiguration);

        // Act
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenJwt);
        var responseClient = await _client.GetAsync(uri);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, responseClient.StatusCode);
        Assert.False(responseClient.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Get_NpsScore_DeveRetornarStatusCodeOk_QuandoOTokenDeAcessoForValido()
    {
        // Arrange
        string uri = "v1/Nps/Score";

        var tokenJwt = new AuthenticationService()
            .SetUsername(_faker.Person.UserName).GenerateToken(_configuration);

        // Act
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenJwt);
        var responseClient = await _client.GetAsync(uri);
        var responseContent = JsonConvert.DeserializeObject<NpsScoreViewModel>(await responseClient.Content.ReadAsStringAsync());

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseClient.StatusCode);
        Assert.True(responseClient.IsSuccessStatusCode);
        Assert.NotNull(responseContent.Score);
    }

    [Fact]
    public async Task Get_NpsSummary_DeveRetornarStatusCodeUnauthorized_QuandoNenhumTokenDeAcessoForEnviado()
    {
        // Arrange
        string uri = "/v1/Nps/Summary";

        // Act
        var clientResponse = await _client.GetAsync(uri);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, clientResponse.StatusCode);
        Assert.False(clientResponse.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Get_NpsSummary_DeveRetornarStatusCodeUnauthorized_QuandoTokenEnviadoForInvalido()
    {
        // Arrange
        string uri = "/v1/Nps/Summary";
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Authentication:SecretKey", Guid.NewGuid().ToString() },
            { "Authentication:Issuer", _faker.Random.String() },
            { "Authentication:Audience", _faker.Random.String() },
            { "Authentication:Expires", "30" },
        };

        var wrongConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var tokenJwt = new AuthenticationService()
            .SetUsername(_faker.Person.UserName).GenerateToken(wrongConfiguration);

        // Act
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenJwt);
        var clientResponse = await _client.GetAsync(uri);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, clientResponse.StatusCode);
        Assert.False(clientResponse.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Get_NpsSummary_DeveRetornarStatusCodeSuccess_QuandoTokenForValido()
    {
        // Arrange
        string uri = "/v1/Nps/Summary";

        var tokenJwt = new AuthenticationService()
            .SetUsername(_faker.Person.UserName).GenerateToken(_configuration);

        // Act
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenJwt);
        var clientResponse = await _client.GetAsync(uri);
        var contentSerialized = await clientResponse.Content.ReadAsStringAsync();
        var contentDeserialized = JsonConvert.DeserializeObject<NpsSummaryViewModel>(contentSerialized);

        // Assert
        Assert.Equal(HttpStatusCode.OK, clientResponse.StatusCode);
        Assert.True(clientResponse.IsSuccessStatusCode);
        Assert.NotNull(contentDeserialized.Detractors);
        Assert.NotNull(contentDeserialized.Neutrals);
        Assert.NotNull(contentDeserialized.Promoters);
        Assert.NotNull(contentDeserialized.NpsScore);
    }
}
