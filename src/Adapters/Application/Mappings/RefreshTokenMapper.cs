using NexoraBackend.Application.Entities;
using NexoraBackend.Core.Domain.Entities;

namespace NexoraBackend.Application.Mappings;

public class RefreshTokenMapper
{
    //Domain to Entity
    public RefreshTokenEntity ToEntity(RefreshToken domain)
    {
        return new RefreshTokenEntity
        {
            Id = domain.Id,
            Token = domain.Token,
            UserId = domain.UserId,
            ExpiresAt = domain.ExpiresAt,
            IsRevoked = domain.IsRevoked
        };
    }

    public RefreshToken ToDomain(RefreshTokenEntity entity)
    {
        return new RefreshToken
        {
            Id = entity.Id,
            Token = entity.Token,
            UserId = entity.UserId,
            ExpiresAt = entity.ExpiresAt,
            IsRevoked = entity.IsRevoked
        };
    }
}