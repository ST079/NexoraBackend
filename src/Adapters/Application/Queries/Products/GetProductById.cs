using AutoMapper;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Behaviors;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;

namespace NexoraBackend.Application.Queries.Products;

public record GetProductByIdQuery(Guid Id)
    : IRequest<Result<ProductDto>>, ICacheableQuery
{
    public string CacheKey => $"product:id:{Id}";
    public TimeSpan? CacheExpiry => TimeSpan.FromMinutes(10);
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    { _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery query, CancellationToken ct)
    {
        var product = await _unitOfWork.Products.GetReviewsAsync(query.Id, ct);
        if (product is null)
            return Result<ProductDto>.Failure(Error.NotFound("Product", query.Id));

        return Result<ProductDto>.Success(_mapper.Map<ProductDto>(product));
    }
}