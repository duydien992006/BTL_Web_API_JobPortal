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

    string[] cacNguon = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new string[0];
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("WebFrontend", policy => policy
            .WithOrigins(cacNguon)
            .AllowAnyHeader()
            .AllowAnyMethod()
            // Cho JavaScript đọc được traceId và tên file khi tải CV
            .WithExposedHeaders("X-Correlation-Id", "Content-Disposition"));
    });

    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    var app = builder.Build();

    app.UseCors("WebFrontend");                            // 1) CORS đứng ĐẦU: trả lời 
    app.UseJobPortalErrorHandling();                       // 2) correlation + log + bắt lỗi
    app.UseMiddleware<JwtValidationMiddleware>();          // 3) kiểm tra JWT
    app.MapReverseProxy();                                 // 4) chuyển tiếp

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