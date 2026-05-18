using AutoMapper;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;

namespace NexoraBackend.Application.Queries.Users;

public record GetUserQuery(Guid UserId) : IRequest<Result<UserDto>>;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetUserQueryHandler(IUnitOfWork uow, IMapper mapper)
    { _uow = uow; _mapper = mapper; }

    public async Task<Result<UserDto>> Handle(GetUserQuery query, CancellationToken ct)
    {
        var user = await _uow.Users.GetByIdAsync(query.UserId, ct);
        if (user is null)
            return Result<UserDto>.Failure(Error.NotFound("User", query.UserId));

        return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
    }
}