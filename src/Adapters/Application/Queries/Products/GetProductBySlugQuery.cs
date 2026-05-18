using AutoMapper;
using MediatR;
using NexoraBackend.Application.Behaviors;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Behaviors;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;

namespace NexoraBackend.Application.Queries.Products;

public record GetProductBySlugQuery(string Slug)
    : IRequest<Result<ProductDto>>, ICacheableQuery
{
    public string CacheKey => $"product:slug:{Slug}";
    public TimeSpan? CacheExpiry => TimeSpan.FromMinutes(10);
}

public class GetProductBySlugQueryHandler : IRequestHandler<GetProductBySlugQuery, Result<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductBySlugQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ProductDto>> Handle(GetProductBySlugQuery query, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetBySlugAsync(query.Slug, cancellationToken);
        if (product is null)
            return Result<ProductDto>.Failure(Error.NotFound("Product", query.Slug));

        return Result<ProductDto>.Success(_mapper.Map<ProductDto>(product));
    }
}