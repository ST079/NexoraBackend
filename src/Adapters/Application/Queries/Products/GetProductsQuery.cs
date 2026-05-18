using AutoMapper;
using MediatR;
using NexoraBackend.Application.Behaviors;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Behaviors;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;

namespace NexoraBackend.Application.Queries.Products;

public record GetProductsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    Guid? CategoryId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null
) : IRequest<Result<PagedList<ProductSummaryDto>>>, ICacheableQuery
{
    public string CacheKey => $"products:{Page}:{PageSize}:{Search}:{CategoryId}:{MinPrice}:{MaxPrice}";
    public TimeSpan? CacheExpiry => TimeSpan.FromMinutes(5);
}

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PagedList<ProductSummaryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PagedList<ProductSummaryDto>>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var paged = await _unitOfWork.Products.GetPagedAsync(
            query.Page, query.PageSize, query.Search,
            query.CategoryId, query.MinPrice, query.MaxPrice,
            isActive: true, cancellationToken);

        var mapped = new PagedList<ProductSummaryDto>(
            paged.Items.Select(p => _mapper.Map<ProductSummaryDto>(p)).ToList(),
            paged.TotalCount, paged.PageNumber, paged.PageSize);

        return Result<PagedList<ProductSummaryDto>>.Success(mapped);
    }
}