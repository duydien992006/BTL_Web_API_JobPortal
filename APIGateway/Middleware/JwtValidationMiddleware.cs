using Microsoft.IdentityModel.Tokens;
using Model;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
namespace APIGateway.Middleware
{
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtValidationMiddleware> _logger;
        private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
        private readonly TokenValidationParameters _validationParameters;

        public JwtValidationMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<JwtValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;

            string secretKey = configuration["AppSettings:Secret"]
                ?? throw new InvalidOperationException("Thiếu AppSettings:Secret trong appsettings.json của Gateway");

            _validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"].ToString();

            // Không gửi token -> cho qua, service phía sau tự quyết định có bắt buộc đăng nhập không
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                await _next(context);
                return;
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();
            string loiTuChoi = null;
            string lyDoKyThuat = null;

            try
            {
                _tokenHandler.ValidateToken(token, _validationParameters, out _);
            }
            catch (SecurityTokenExpiredException)
            {
                loiTuChoi = "Token đã hết hạn, vui lòng đăng nhập lại";
                lyDoKyThuat = "Expired";
            }
            catch (Exception ex)
            {
                loiTuChoi = "Token không hợp lệ";
                lyDoKyThuat = ex.GetType().Name;
            }

            if (loiTuChoi != null)
            {
                // KHÔNG log nội dung token (là thông tin nhạy cảm)
                _logger.LogWarning("Gateway từ chối token ({LyDo}) khi gọi {Method} {Path}",
                    lyDoKyThuat, context.Request.Method, context.Request.Path.Value);

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new PhanHoiModel
                {
                    ThanhCong = false,
                    ThongDiep = loiTuChoi,
                    DuLieu = new { traceId = context.TraceIdentifier }
                });
                return;
            }

            await _next(context); // nằm NGOÀI try
        }
    }
}
