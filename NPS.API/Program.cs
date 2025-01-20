using NPS.API.Extensions;
using NPS.Application.NpsCQ.Queries;
using NPS.Core.Interfaces.Repositorios;
using NPS.Infrastructure.Data.Queries;
using NPS.Infrastructure.Repositories;
using NPS.Infrastructure.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

builder.AddServices();
builder.AddSqlServer();
builder.AddMediatR();

builder.Services.AddScoped(typeof(INpsRepository), typeof(NpsRepository));
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
builder.Services.AddScoped(typeof(INpsQuery), typeof(NpsQuery));
builder.Services.AddScoped(typeof(INpsQueryService), typeof(NpsQueryService));

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
    await unitOfWork.Commit();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
