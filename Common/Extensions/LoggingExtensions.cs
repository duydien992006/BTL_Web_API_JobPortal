using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.IO;

namespace Common.Extensions
{
    public static class LoggingExtensions
    {
        private const string MauLog =
            "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] [{Service}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}";

        public static WebApplicationBuilder AddJobPortalLogging(this WebApplicationBuilder builder, string tenService)
        {
            // Mặc định: thư mục Logs nằm trong project. Có thể đổi bằng AppSettings:LogDirectory
            string thuMucLog = builder.Configuration["AppSettings:LogDirectory"];
            if (string.IsNullOrWhiteSpace(thuMucLog))
                thuMucLog = Path.Combine(builder.Environment.ContentRootPath, "Logs");

            builder.Host.UseSerilog((context, services, cauHinh) =>
            {
                cauHinh
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .MinimumLevel.Override("Yarp", LogEventLevel.Warning)
                    // .NET tự log exception 1 lần nữa -> tắt để tránh log trùng (handler của mình đã log)
                    .MinimumLevel.Override("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware", LogEventLevel.Fatal)

                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("CorrelationId", "-")   // giá trị mặc định cho log lúc khởi động
                    .Enrich.WithProperty("Service", tenService)
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)

                    // 1) Console (xem nhanh khi bật console)
                    .WriteTo.Console(outputTemplate: MauLog)

                    // 2) File text dễ đọc, xoay vòng theo ngày, giữ 14 ngày
                    .WriteTo.File(
                        path: Path.Combine(thuMucLog, tenService + "-.log"),
                        outputTemplate: MauLog,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 14,
                        shared: true,
                        encoding: Encoding.UTF8)

                    // 3) File JSON có cấu trúc (mỗi dòng 1 JSON) - để máy phân tích/tìm kiếm
                    .WriteTo.File(
                        new CompactJsonFormatter(),
                        Path.Combine(thuMucLog, tenService + "-.json"),
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 14,
                        shared: true)

                    // Đặt CUỐI để appsettings.json ("Serilog": {...}) có thể ghi đè mức log mà không cần build lại
                    .ReadFrom.Configuration(context.Configuration);
            });

            return builder;
        }
    }
}
