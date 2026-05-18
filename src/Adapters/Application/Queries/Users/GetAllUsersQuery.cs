using AutoMapper;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Domain;


namespace NexoraBackend.Application.Queries.Users;

public record GetAllUsersQuery(int Page = 1, int PageSize = 20)
    : IRequest<Result<PagedList<UserDto>>>;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<PagedList<UserDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    { _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<Result<PagedList<UserDto>>> Handle(GetAllUsersQuery query, CancellationToken ct)
    {
        // Use IQueryable so EF Core pushes COUNT and SKIP/TAKE to the database.
        // Loading all users into memory would be catastrophic at scale.
        var queryable = _unitOfWork.Users.Query()
            .OrderBy(u => u.CreatedAt);

        var paged = await PagedList<User>.CreateAsync(queryable, query.Page, query.PageSize, ct);

        return Result<PagedList<UserDto>>.Success(new PagedList<UserDto>(
            paged.Items.Select(u => _mapper.Map<UserDto>(u)).ToList(),
            paged.TotalCount, paged.PageNumber, paged.PageSize));
    }
}