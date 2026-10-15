using Common.Exceptions;
using Common.Problems;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;



namespace Common.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private const string ThongDiepMacDinh = "Hệ thống đang gặp sự cố, vui lòng thử lại sau";

        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            string method = httpContext.Request.Method;
            string path = httpContext.Request.Path;

            // Client tự hủy request: không phải lỗi hệ thống
            if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
            {
                _logger.LogInformation("Client đã hủy request {Method} {Path}", method, path);
                return true;
            }

            var (status, detail, maLoi) = PhanLoai(exception);

            // Log đúng 1 lần tại đây (các tầng dưới chỉ throw, không log lại)
            if (status >= 500)
                _logger.LogError(exception, "Lỗi hệ thống {StatusCode} ({MaLoi}) khi xử lý {Method} {Path}",
                    status, maLoi, method, path);
            else
                _logger.LogWarning("Yêu cầu bị từ chối {StatusCode} ({MaLoi}) tại {Method} {Path}: {LyDo}",
                    status, maLoi, method, path, exception.Message);

            var problem = new ProblemDetails { Status = status, Detail = detail };
            problem.Extensions["maLoi"] = maLoi;

            // Chỉ lộ stack trace khi chạy Development. KHÔNG lộ ở Production
            if (_env.IsDevelopment())
                problem.Extensions["chiTiet"] = exception.ToString();

            await ProblemDetailsHelper.GhiAsync(httpContext, problem);
            return true;
        }

        private static (int Status, string Detail, string MaLoi) PhanLoai(Exception ex)
        {
            if (ex is AppException app)
                return (app.StatusCode, app.Message, app.MaLoi);

            var sql = ex as SqlException ?? ex.InnerException as SqlException;
            if (sql != null)
                return PhanLoaiSql(sql);

            if (ex is BadHttpRequestException bad)
                return (bad.StatusCode, "Yêu cầu gửi lên không hợp lệ", "YEU_CAU_KHONG_HOP_LE");

            if (ex is FormatException)
                return (400, "Dữ liệu gửi lên không đúng định dạng", "SAI_DINH_DANG");

            // NullReferenceException và mọi lỗi không lường trước rơi vào đây
            return (500, ThongDiepMacDinh, "LOI_HE_THONG");
        }

        private static (int Status, string Detail, string MaLoi) PhanLoaiSql(SqlException sql)
        {
            switch (sql.Number)
            {
                case 547:
                    return sql.Message.Contains("DELETE")
                        ? (409, "Không thể xóa vì dữ liệu đang được sử dụng ở nơi khác", "DB_RANG_BUOC_KHOA_NGOAI")
                        : (409, "Dữ liệu tham chiếu không tồn tại hoặc không hợp lệ", "DB_RANG_BUOC_KHOA_NGOAI");

                case 2601:
                case 2627:
                    return (409, "Dữ liệu bị trùng lặp, đã tồn tại bản ghi tương tự", "DB_TRUNG_DU_LIEU");

                case 1205:
                    return (503, "Hệ thống đang bận, vui lòng thử lại sau ít giây", "DB_DEADLOCK");

                case -2:
                    return (504, "Cơ sở dữ liệu phản hồi quá chậm, vui lòng thử lại", "DB_TIMEOUT");

                case 53:
                case 4060:
                case 18456:
                case -1:
                case 2:
                case 258:
                    return (503, "Không kết nối được tới cơ sở dữ liệu", "DB_KHONG_KET_NOI");

                default:
                    return (500, ThongDiepMacDinh, "DB_LOI_KHAC");
            }
        }
    }
}
