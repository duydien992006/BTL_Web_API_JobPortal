// ============ APIGateway/Program.cs ============
using APIGateway.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseMiddleware<JwtValidationMiddleware>();

app.MapReverseProxy();

app.Run();