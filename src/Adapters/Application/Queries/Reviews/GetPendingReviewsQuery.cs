
using AutoMapper;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;


namespace NexoraBackend.Application.Queries.Reviews;

public record GetPendingReviewsQuery(int Page = 1, int PageSize = 20)
    : IRequest<Result<PagedList<ReviewDto>>>;

public class GetPendingReviewsQueryHandler
    : IRequestHandler<GetPendingReviewsQuery, Result<PagedList<ReviewDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetPendingReviewsQueryHandler(IUnitOfWork uow, IMapper mapper)
    { _uow = uow; _mapper = mapper; }

    public async Task<Result<PagedList<ReviewDto>>> Handle(GetPendingReviewsQuery query, CancellationToken ct)
    {
        var paged = await _uow.Reviews.GetPendingAsync(query.Page, query.PageSize, ct);
        return Result<PagedList<ReviewDto>>.Success(new PagedList<ReviewDto>(
            paged.Items.Select(r => _mapper.Map<ReviewDto>(r)).ToList(),
            paged.TotalCount, paged.PageNumber, paged.PageSize));
    }
}