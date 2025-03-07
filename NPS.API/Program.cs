using NPS.API.Extensions;
using NPS.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddServices();
builder.AddDependencyInjectionServices();
builder.AddSqlServer();
builder.AddMediatR();
builder.AddJwtAuth();

builder.Services.AddAuthorization();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<UnitOfWorkMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program {}
