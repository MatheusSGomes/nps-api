using Microsoft.EntityFrameworkCore;
using NPS.Application.NpsCQ.Handlers;
using NPS.Infrastructure.Persistence;

namespace NPS.API.Extensions;

public static class BuilderExtensions
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    public static void AddSqlServer(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services.AddDbContext<NpsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    }

    public static void AddMediatR(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(NpsCommandHandler).Assembly));
    }

    public static void AddJwtAuth(this WebApplicationBuilder builder)
    {}
}
