using Microsoft.EntityFrameworkCore;
using NPS.Application.NpsCQ.Handlers;
using NPS.Application.NpsCQ.Queries;
using NPS.Core.Interfaces.Repositorios;
using NPS.Infrastructure.Data.Queries;
using NPS.Infrastructure.Persistence;
using NPS.Infrastructure.Repositories;
using NPS.Infrastructure.UnitOfWork;

namespace NPS.API.Extensions;

public static class BuilderExtensions
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    public static void AddDependencyInjectionServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped(typeof(INpsRepository), typeof(NpsRepository));
        builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
        builder.Services.AddScoped(typeof(INpsQuery), typeof(NpsQuery));
        builder.Services.AddScoped(typeof(INpsQueryService), typeof(NpsQueryService));
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
