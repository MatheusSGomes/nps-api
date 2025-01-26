using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NPS.Application.NpsCQ.Handlers;
using NPS.Application.NpsCQ.Queries;
using NPS.Application.Services;
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
        builder.Services.AddScoped(typeof(IAuthenticationService), typeof(AuthenticationService));
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
    {
        var configuration = builder.Configuration;

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "nps.com.br",
                    ValidAudience = "nps.com.br",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Authentication:SecretKey")!))
                };
            });
    }
}
