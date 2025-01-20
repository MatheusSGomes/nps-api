using NPS.API.Extensions;
using NPS.Infrastructure.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

builder.AddServices();
builder.AddDependencyInjectionServices();
builder.AddSqlServer();
builder.AddMediatR();

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
