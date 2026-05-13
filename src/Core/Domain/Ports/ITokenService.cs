using System.Security.Claims;
using NexoraBackend.Core.Domain.Entities;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();

    ClaimsPrincipal?GetPrincipalFromExpiredToken(string token);
}