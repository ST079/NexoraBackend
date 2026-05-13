using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using NexoraBackend.Application.Mappings;
using NexoraBackend.Core.Domain.Entities;
using NexoraBackend.Core.Domain.Ports;
using NexoraBackend.Infrastructure.Persistence;

namespace NexoraBackend.Infrastructure.Repositories;


public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _dbContext;
    private readonly RefreshTokenMapper _mapper;

    public RefreshTokenRepository(AppDbContext dbContext, RefreshTokenMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task CreateAsync(RefreshToken refreshToken)
    {
        await _dbContext.RefreshTokens.AddAsync(_mapper.ToEntity(refreshToken));
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {

        var entity = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
        if (entity == null)
            return null;
        return _mapper.ToDomain(entity);
    }

    public async Task<bool> RevokeAsync(string token)
    {
        var refreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            return true;
        }
        return false;
    }
}
