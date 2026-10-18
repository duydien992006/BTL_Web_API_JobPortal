// ============ Program.cs (4 project API) ============
using BLL;
using BLL.Interfaces;
using Common.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

// Bootstrap logger: ghi lại lỗi xảy ra TRƯỚC khi hệ thống logging chính được dựng xong
// (VD: "Some services are not able to be constructed")
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/startup-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddJobPortalLogging("APIQuanTriVien");          // <-- ĐỔI TÊN THEO TỪNG PROJECT

    builder.Services.AddControllers().AddJobPortalMvc();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddJobPortalErrorHandling();          // <-- MỚI

    string secretKey = builder.Configuration["AppSettings:Secret"]
        ?? throw new InvalidOperationException("Thiếu AppSettings:Secret trong appsettings.json");

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                string idClaim = context.Principal?.FindFirst("Id")?.Value;
                int nguoiDungId;
                if (int.TryParse(idClaim, out nguoiDungId))
                {
                    var dichVu = context.HttpContext.RequestServices.GetRequiredService<ITaiKhoanKhoaService>();
                    if (await dichVu.BiKhoaAsync(nguoiDungId))
                        context.Fail("Tài khoản đã bị khóa");
                }
            }
        };
    });
    builder.Services.AddAuthorization();

    builder.Services.AddJobPortalServices();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseJobPortalErrorHandling();                       // <-- MỚI: PHẢI đứng trước UseAuthentication

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("Khởi động thành công, đang lắng nghe request");
    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Ứng dụng không khởi động được");
}
finally
{
    Log.CloseAndFlush();
}