using Common.Handlers;
using Common.Middleware;
using Common.Problems;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Model;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extensions
{
    public static class ErrorHandlingExtensions
    {
        public static IServiceCollection AddJobPortalErrorHandling(this IServiceCollection services)
        {
            // Mọi ProblemDetails do .NET tự sinh cũng được điền type/title/instance/traceId theo cùng 1 quy tắc
            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = ctx =>
                    ProblemDetailsHelper.DienMacDinh(ctx.HttpContext, ctx.ProblemDetails);
            });

            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                // return NotFound()/BadRequest() không body -> đi tiếp tới UseStatusCodePages
                options.SuppressMapClientErrors = true;

                // Lỗi validate model / JSON sai cú pháp -> ValidationProblemDetails (RFC 7807 + trường "errors")
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

                    var problem = new ValidationProblemDetails(loi)
                    {
                        Status = 400,
                        Title = "Dữ liệu gửi lên không hợp lệ",
                        Detail = "Một hoặc nhiều trường dữ liệu không hợp lệ"
                    };
                    problem.Extensions["maLoi"] = "DU_LIEU_KHONG_HOP_LE";
                    ProblemDetailsHelper.DienMacDinh(ctx.HttpContext, problem);

                    return new ObjectResult(problem)
                    {
                        StatusCode = 400,
                        ContentTypes = { ProblemDetailsHelper.ContentType }
                    };
                };
            });

            return services;
        }

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

            // 401/403/404/502... mà body rỗng -> ProblemDetails
            app.UseStatusCodePages(ctx =>
            {
                int status = ctx.HttpContext.Response.StatusCode;
                return ProblemDetailsHelper.GhiAsync(ctx.HttpContext, new ProblemDetails
                {
                    Status = status,
                    Detail = ThongDiepTheoMa(status)
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
