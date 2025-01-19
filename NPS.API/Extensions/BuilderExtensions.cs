namespace NPS.API.Extensions;

public static class BuilderExtensions
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    public static void AddJwtAuth(this WebApplicationBuilder builder)
    {}
}
