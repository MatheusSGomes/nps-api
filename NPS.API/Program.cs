using Microsoft.EntityFrameworkCore;
using NPS.Application.NpsCQ.Handlers;
using NPS.Core.Interfaces.Repositorios;
using NPS.Infrastructure.Persistence;
using NPS.Infrastructure.Repositories;
using NPS.Infrastructure.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;

builder.Services.AddDbContext<NpsDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(NpsCommandHandler).Assembly));

builder.Services.AddScoped(typeof(INpsRepository), typeof(NpsRepository));
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    // Após invocar toda a aplicação
    await next.Invoke();

    // Recupero o UnitOfWork para aplicar o commit (SaveChanges)
    var unitOfWork = (IUnitOfWork)context.RequestServices.GetService(typeof(IUnitOfWork));
    unitOfWork.Commit();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
