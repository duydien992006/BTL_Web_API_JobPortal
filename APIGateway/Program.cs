// ============ APIGateway/Program.cs ============
using APIGateway.Middleware;
using Common.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/startup-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddJobPortalLogging("APIGateway");
    builder.Services.AddJobPortalErrorHandling();

    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    var app = builder.Build();

    app.UseJobPortalErrorHandling();                       // 1) correlation + log + bắt lỗi
    app.UseMiddleware<JwtValidationMiddleware>();          // 2) kiểm tra JWT
    app.MapReverseProxy();                                 // 3) chuyển tiếp

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Gateway không khởi động được");
}
finally
{
    Log.CloseAndFlush();
}