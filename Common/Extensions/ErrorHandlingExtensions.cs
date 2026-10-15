using Common.Handlers;
using Common.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Model;
using Serilog;
using Serilog.Events;
using System.Security.Claims;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extensions
{
    public static class ErrorHandlingExtensions
    {
        // Gọi trong phần đăng ký service (trước builder.Build())
        public static IServiceCollection AddJobPortalErrorHandling(this IServiceCollection services)
        {
            services.AddProblemDetails();                       // .NET 8 yêu cầu khi dùng UseExceptionHandler()
            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                // Để return NotFound()/BadRequest() không body đi tiếp tới UseStatusCodePages -> JSON thống nhất
                options.SuppressMapClientErrors = true;

                // Lỗi validate model / JSON sai cú pháp -> cùng định dạng PhanHoiModel
                options.InvalidModelStateResponseFactory = ctx =>
                {
                    var loi = ctx.ModelState
                        .Where(e => e.Value != null && e.Value.Errors.Count > 0)
                        .ToDictionary(
                            e => e.Key,
                            e => e.Value.Errors
                                .Select(x => !string.IsNullOrEmpty(x.ErrorMessage)
                                    ? x.ErrorMessage
                                    : (x.Exception?.Message ?? "Giá trị không hợp lệ"))
                                .ToArray());

                    var logger = ctx.HttpContext.RequestServices
                        .GetRequiredService<ILoggerFactory>().CreateLogger("ModelValidation");
                    logger.LogWarning("Dữ liệu gửi lên không hợp lệ tại {Path}: {@LoiValidation}",
                        ctx.HttpContext.Request.Path.Value, loi);

                    return new BadRequestObjectResult(new PhanHoiModel
                    {
                        ThanhCong = false,
                        ThongDiep = "Dữ liệu gửi lên không hợp lệ",
                        DuLieu = new { traceId = ctx.HttpContext.TraceIdentifier, loi }
                    });
                };
            });

            return services;
        }

        // Gọi trong pipeline, SAU Swagger và TRƯỚC UseAuthentication
        public static IApplicationBuilder UseJobPortalErrorHandling(this IApplicationBuilder app)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();

            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} => {StatusCode} trong {Elapsed:0.0} ms";

                options.GetLevel = (ctx, elapsed, ex) =>
                {
                    if (ctx.Request.Path.StartsWithSegments("/swagger")) return LogEventLevel.Verbose;
                    if (ctx.Response.StatusCode >= 500) return LogEventLevel.Error;
                    if (ctx.Response.StatusCode >= 400) return LogEventLevel.Warning;
                    return LogEventLevel.Information;
                };

                // Thêm property vào dòng log "request completed"
                // (callback chạy SAU khi pipeline xong nên User đã được xác thực)
                options.EnrichDiagnosticContext = (diag, ctx) =>
                {
                    diag.Set("ClientIP", ctx.Connection.RemoteIpAddress?.ToString());

                    string userId = ctx.User.FindFirst("Id")?.Value;
                    if (userId != null) diag.Set("UserId", userId);

                    string vaiTro = ctx.User.FindFirst(ClaimTypes.Role)?.Value;
                    if (vaiTro != null) diag.Set("VaiTro", vaiTro);
                };
            });

            app.UseExceptionHandler();

            // 401/403/404/502... mà body rỗng -> trả JSON PhanHoiModel
            app.UseStatusCodePages(async ctx =>
            {
                var res = ctx.HttpContext.Response;
                await res.WriteAsJsonAsync(new PhanHoiModel
                {
                    ThanhCong = false,
                    ThongDiep = ThongDiepTheoMa(res.StatusCode),
                    DuLieu = new { traceId = ctx.HttpContext.TraceIdentifier }
                });
            });

            return app;
        }

        private static string ThongDiepTheoMa(int statusCode)
        {
            switch (statusCode)
            {
                case 400: return "Yêu cầu không hợp lệ";
                case 401: return "Bạn chưa đăng nhập hoặc token không hợp lệ";
                case 403: return "Bạn không có quyền thực hiện chức năng này";
                case 404: return "Không tìm thấy tài nguyên yêu cầu";
                case 405: return "Phương thức HTTP không được hỗ trợ";
                case 415: return "Định dạng dữ liệu gửi lên không được hỗ trợ";
                case 502: return "Gateway không kết nối được tới dịch vụ phía sau";
                case 503: return "Dịch vụ tạm thời không khả dụng";
                case 504: return "Dịch vụ phía sau phản hồi quá chậm";
                default: return "Yêu cầu không thể xử lý";
            }
        }
    }
}
