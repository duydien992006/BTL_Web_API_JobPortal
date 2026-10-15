using Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
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

            // Client tự hủy request (đóng tab, bấm Cancel) -> không phải lỗi hệ thống
            if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
            {
                _logger.LogInformation("Client đã hủy request {Method} {Path}", method, path);
                return true;
            }

            var (statusCode, thongDiep) = PhanLoai(exception);

            // Log ĐÚNG 1 LẦN tại đây. Các tầng dưới (DAL/BLL) chỉ throw, không log lại.
            if (statusCode >= 500)
            {
                _logger.LogError(exception, "Lỗi hệ thống {StatusCode} khi xử lý {Method} {Path}",
                    statusCode, method, path);
            }
            else
            {
                _logger.LogWarning("Yêu cầu bị từ chối {StatusCode} ({LoaiLoi}) tại {Method} {Path}: {LyDo}",
                    statusCode, exception.GetType().Name, method, path, exception.Message);
            }

            var duLieu = new Dictionary<string, object> { ["traceId"] = httpContext.TraceIdentifier };
            if (_env.IsDevelopment())
            {
                // Chỉ lộ chi tiết khi chạy Development, KHÔNG lộ ở Production
                duLieu["chiTiet"] = exception.ToString();
            }

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(new PhanHoiModel
            {
                ThanhCong = false,
                ThongDiep = thongDiep,
                DuLieu = duLieu
            }, cancellationToken);

            return true; // đã xử lý xong, không chuyển cho handler khác
        }

        private static (int StatusCode, string ThongDiep) PhanLoai(Exception ex)
        {
            if (ex is AppException app)
                return (app.StatusCode, app.Message);

            var sql = ex as SqlException ?? ex.InnerException as SqlException;
            if (sql != null)
                return PhanLoaiSql(sql);

            if (ex is BadHttpRequestException bad)
                return (bad.StatusCode, "Yêu cầu gửi lên không hợp lệ");

            // VD: int.Parse trên dữ liệu client gửi (danhSachKyNangId = "abc")
            if (ex is FormatException)
                return (400, "Dữ liệu gửi lên không đúng định dạng");

            return (500, ThongDiepMacDinh);
        }

        private static (int StatusCode, string ThongDiep) PhanLoaiSql(SqlException sql)
        {
            switch (sql.Number)
            {
                case 547: // vi phạm FOREIGN KEY / CHECK
                    return sql.Message.Contains("DELETE")
                        ? (409, "Không thể xóa vì dữ liệu đang được sử dụng ở nơi khác")
                        : (409, "Dữ liệu tham chiếu không tồn tại hoặc không hợp lệ");

                case 2601: // trùng unique index
                case 2627: // trùng primary key / unique constraint
                    return (409, "Dữ liệu bị trùng lặp, đã tồn tại bản ghi tương tự");

                case 1205: // deadlock
                    return (503, "Hệ thống đang bận, vui lòng thử lại sau ít giây");

                case -2: // timeout
                    return (504, "Cơ sở dữ liệu phản hồi quá chậm, vui lòng thử lại");

                case 53:    // không tìm thấy server
                case 4060:  // không mở được database
                case 18456: // đăng nhập SQL thất bại
                case -1:
                case 2:
                case 258:
                    return (503, "Không kết nối được tới cơ sở dữ liệu");

                default:
                    return (500, ThongDiepMacDinh);
            }
        }
    }
}
