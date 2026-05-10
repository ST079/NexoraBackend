using NexoraBackend.Core.Domain.Entities;

public interface ITokenService
{
    string GenerateToken(User user);
}