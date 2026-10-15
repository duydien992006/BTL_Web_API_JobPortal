using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Common.Middleware
{
    public class CorrelationIdMiddleware
    {
        public const string HeaderName = "X-Correlation-Id";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string correlationId = context.Request.Headers[HeaderName].FirstOrDefault();

            // Chỉ chấp nhận mã ngắn, ký tự an toàn (chống log injection)
            bool hopLe = !string.IsNullOrWhiteSpace(correlationId)
                         && correlationId.Length <= 64
                         && correlationId.All(c => char.IsLetterOrDigit(c) || c == '-');

            if (!hopLe)
            {
                correlationId = Guid.NewGuid().ToString("N");
                // Ghi vào request header để YARP forward xuống các service phía sau
                context.Request.Headers[HeaderName] = correlationId;
            }

            // TraceIdentifier = CorrelationId -> traceId trả về cho client khớp với log
            context.TraceIdentifier = correlationId;

            context.Response.OnStarting(() =>
            {
                context.Response.Headers[HeaderName] = correlationId;
                return Task.CompletedTask;
            });

            // Mọi log ghi trong phạm vi request này đều tự có property CorrelationId
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}
