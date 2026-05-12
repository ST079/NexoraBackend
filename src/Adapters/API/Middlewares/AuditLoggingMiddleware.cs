using System.Security.Claims;
using NexoraBackend.Core.Domain.Entities;
using NexoraBackend.Core.Domain.Ports;

namespace NexoraBackend.API.Middlewares;

public class AuditloggingMiddleware
{
    private readonly RequestDelegate _next;

    public AuditloggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuditLogRepository auditLogRepository)
    {

        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var path = context.Request.Path.Value?.ToLower();

        var ignorePaths = new[]
        {
            "/graphql",
            "/swagger",
            "/health",
            "/favicon.svg",
            "/metrics",
            "/favicon.ico",
            "/.well-known",
        };

        if (ignorePaths.Any(p => path != null && path.StartsWith(p)))
        {
            await _next(context);
            return;
        }

        var log = new AuditLog
        {
            UserId = string.IsNullOrEmpty(userId) ? null : Guid.Parse(userId),
            Action = context.Request.Path,
            IpAddress = context.Connection.RemoteIpAddress?.ToString(),
        };

        await auditLogRepository.AddAsync(log);

        await _next(context);
    }

}