namespace NexoraBackend.API.Extensions;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext context)
    {
        var claim = context.User.FindFirst(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirst(JwtRegisteredClaimNames.Sub)
            ?? throw new UnauthorizedAccessException("User ID claim not found.");
        return Guid.Parse(claim.Value);
    }

    public static string GetUserRole(this HttpContext context) =>
        context.User.FindFirst(ClaimTypes.Role)?.Value ?? "Customer";

    public static bool IsAdmin(this HttpContext context) =>
        context.GetUserRole() is "Admin";
}
