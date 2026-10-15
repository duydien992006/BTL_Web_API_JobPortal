using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.Problems
{
    public static class ProblemDetailsHelper
    {
        public const string ContentType = "application/problem+json";

        // status -> (mục trong RFC 9110, tiêu đề ngắn gọn, ổn định cho từng loại lỗi)
        private static readonly Dictionary<int, (string Muc, string TieuDe)> Bang =
            new Dictionary<int, (string, string)>
            {
                [400] = ("15.5.1", "Yêu cầu không hợp lệ"),
                [401] = ("15.5.2", "Chưa xác thực"),
                [403] = ("15.5.4", "Không có quyền truy cập"),
                [404] = ("15.5.5", "Không tìm thấy"),
                [405] = ("15.5.6", "Phương thức không được hỗ trợ"),
                [409] = ("15.5.10", "Xung đột dữ liệu"),
                [415] = ("15.5.16", "Định dạng dữ liệu không được hỗ trợ"),
                [422] = ("15.5.21", "Không thể xử lý dữ liệu"),
                [500] = ("15.6.1", "Lỗi hệ thống"),
                [502] = ("15.6.3", "Lỗi cổng kết nối"),
                [503] = ("15.6.4", "Dịch vụ tạm thời không khả dụng"),
                [504] = ("15.6.5", "Dịch vụ phản hồi quá chậm"),
            };

        // Điền các trường dùng chung. Trường nào đã có giá trị thì giữ nguyên.
        public static void DienMacDinh(HttpContext ctx, ProblemDetails pd)
        {
            int status = pd.Status ?? 500;
            pd.Status = status;

            if (pd.Type == null)
            {
                pd.Type = Bang.TryGetValue(status, out var v)
                    ? "https://tools.ietf.org/html/rfc9110#section-" + v.Muc
                    : "about:blank";
            }
            if (pd.Title == null)
            {
                pd.Title = Bang.TryGetValue(status, out var v2) ? v2.TieuDe : "Yêu cầu không thể xử lý";
            }

            // instance: nguồn gốc của lỗi, ở đây là đường dẫn đang gọi
            if (pd.Instance == null) pd.Instance = ctx.Request.Path.Value;

            // traceId = CorrelationId, khớp với log. Nằm ở trường ngoài cùng, không còn nằm trong "duLieu"
            pd.Extensions["traceId"] = ctx.TraceIdentifier;
        }

        // Ghi response với đúng Content-Type: application/problem+json
        public static Task GhiAsync(HttpContext ctx, ProblemDetails pd)
        {
            DienMacDinh(ctx, pd);
            ctx.Response.StatusCode = pd.Status.Value;
            // Truyền pd.GetType() để ValidationProblemDetails cũng xuất đủ trường "errors"
            return ctx.Response.WriteAsJsonAsync(pd, pd.GetType(), (JsonSerializerOptions)null, ContentType);
        }
    }
}
