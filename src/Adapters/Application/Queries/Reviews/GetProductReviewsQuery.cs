// src/Adapters/NexoraBackend.Application/Queries/Products/GetProductReviewsQuery.cs
using AutoMapper;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Behaviors;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Enums;

namespace NexoraBackend.Application.Queries.Reviews;

public record GetProductReviewsQuery(Guid ProductId, int Page = 1, int PageSize = 10)
    : IRequest<Result<PagedList<ReviewSummaryDto>>>, ICacheableQuery
{
    public string CacheKey => $"reviews:product:{ProductId}:{Page}:{PageSize}";
    public TimeSpan? CacheExpiry => TimeSpan.FromMinutes(5);
}

public class GetProductReviewsQueryHandler
    : IRequestHandler<GetProductReviewsQuery, Result<PagedList<ReviewSummaryDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetProductReviewsQueryHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<Result<PagedList<ReviewSummaryDto>>> Handle(GetProductReviewsQuery query, CancellationToken ct)
    {
        var paged = await _uow.Reviews.GetByProductAsync(
            query.ProductId, query.Page, query.PageSize, ReviewStatus.Approved, ct);

        return Result<PagedList<ReviewSummaryDto>>.Success(new PagedList<ReviewSummaryDto>(
            paged.Items.Select(r => _mapper.Map<ReviewSummaryDto>(r)).ToList(),
            paged.TotalCount, paged.PageNumber, paged.PageSize));
    }
}