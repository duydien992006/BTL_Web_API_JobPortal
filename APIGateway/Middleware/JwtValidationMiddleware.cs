using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
namespace APIGateway.Middleware
{
    // Middleware này đóng vai trò "người gác cổng đầu tiên" tại API Gateway,
    // đúng theo kiến trúc trong sơ đồ: Gateway nhận Access Token, kiểm tra JWT
    // trước khi chuyển tiếp xuống từng Microservice (REST API) phía sau.
    //
    // LƯU Ý QUAN TRỌNG: Middleware này CHỈ kiểm tra chữ ký + hạn dùng của Token
    // (xác thực Token có hợp lệ không), KHÔNG kiểm tra Role (phân quyền theo vai trò).
    // Việc kiểm tra Role vẫn do từng Microservice tự đảm nhiệm thông qua
    // [Authorize(Roles = "...")] đã viết sẵn ở Controller — đúng như chú thích
    // "JWT processing at each microservice" trong sơ đồ.
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _secretKey;

        public JwtValidationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _secretKey = configuration["AppSettings:Secret"]
                ?? throw new InvalidOperationException("Thiếu AppSettings:Secret trong appsettings.json của Gateway");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"].ToString();

            // Route không gửi kèm Token (VD: đăng ký, đăng nhập, xem danh sách kỹ năng công khai...)
            // -> cho qua luôn, để Microservice phía sau tự quyết định route đó có bắt buộc đăng nhập hay không
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                await _next(context);
                return;
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                // Chỉ kiểm tra: đúng chữ ký (do đúng Secret ký ra) + còn hạn sử dụng
                tokenHandler.ValidateToken(token, validationParameters, out _);

                // Token hợp lệ -> cho request tiếp tục đi xuống YARP để định tuyến tới Microservice
                await _next(context);
            }
            catch (SecurityTokenExpiredException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    thanhCong = false,
                    thongDiep = "Token đã hết hạn, vui lòng đăng nhập lại"
                });
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    thanhCong = false,
                    thongDiep = "Token không hợp lệ"
                });
            }
        }
    }
}
