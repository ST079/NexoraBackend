using System.Security.Claims;

namespace NexoraBackend.Application.Interfaces.Services;


public interface IAuthService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
    string GenerateAccessToken(Guid userId, string email);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}