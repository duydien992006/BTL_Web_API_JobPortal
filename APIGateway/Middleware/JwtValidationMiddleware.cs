using Common.Problems;
using Microsoft.AspNetCore.Mvc;
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

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                await _next(context);
                return;
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();
            string detail = null, maLoi = null, lyDoKyThuat = null;

            try
            {
                _tokenHandler.ValidateToken(token, _validationParameters, out _);
            }
            catch (SecurityTokenExpiredException)
            {
                detail = "Token đã hết hạn, vui lòng đăng nhập lại";
                maLoi = "TOKEN_HET_HAN";
                lyDoKyThuat = "Expired";
            }
            catch (Exception ex)
            {
                detail = "Token không hợp lệ";
                maLoi = "TOKEN_KHONG_HOP_LE";
                lyDoKyThuat = ex.GetType().Name;
            }

            if (detail != null)
            {
                // KHÔNG log nội dung token
                _logger.LogWarning("Gateway từ chối token ({LyDo}) khi gọi {Method} {Path}",
                    lyDoKyThuat, context.Request.Method, context.Request.Path.Value);

                // RFC 6750: 401 phải kèm header WWW-Authenticate
                context.Response.Headers["WWW-Authenticate"] = "Bearer error=\"invalid_token\"";

                var problem = new ProblemDetails { Status = 401, Detail = detail };
                problem.Extensions["maLoi"] = maLoi;
                await ProblemDetailsHelper.GhiAsync(context, problem);
                return;
            }

            await _next(context);
        }
    }
}
