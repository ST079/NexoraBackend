// src/Adapters/NexoraBackend.Application/Queries/Categories/GetCategoriesQuery.cs
using AutoMapper;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Behaviors;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;

namespace NexoraBackend.Application.Queries.Categories;

public record GetCategoriesQuery(bool RootOnly = true)
    : IRequest<Result<IReadOnlyList<CategoryDto>>>, ICacheableQuery
{
    public string CacheKey => $"categories:root={RootOnly}";
    public TimeSpan? CacheExpiry => TimeSpan.FromMinutes(30);
}

public class GetCategoriesQueryHandler
    : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyList<CategoryDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetCategoriesQueryHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<Result<IReadOnlyList<CategoryDto>>> Handle(GetCategoriesQuery query, CancellationToken ct)
    {
        var categories = query.RootOnly
            ? await _uow.Categories.GetRootCategoriesAsync(ct)
            : await _uow.Categories.GetAllAsync(ct);

        return Result<IReadOnlyList<CategoryDto>>.Success(
            categories.Select(c => _mapper.Map<CategoryDto>(c)).ToList());
    }
}