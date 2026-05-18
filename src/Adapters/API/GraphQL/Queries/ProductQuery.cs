using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Queries.Products;

namespace NexoraBackend.API.GraphQL.Queries;

[QueryType]
public class ProductQueries
{
    // Hot Chocolate automatically handles pagination, filtering, and sorting
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<ProductSummaryDto>> GetProducts(
        [Service] IMediator mediator,
        int page = 1,
        int pageSize = 20,
        string? search = null,
        Guid? categoryId = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(
            new GetProductsQuery(page, pageSize, search, categoryId), ct);
        return result.IsSuccess ? result.Value!.Items : [];
    }

    public async Task<ProductDto?> GetProductBySlug(
        string slug,
        [Service] IMediator mediator,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetProductBySlugQuery(slug), ct);
        return result.IsSuccess ? result.Value : null;
    }

    public async Task<ProductDto?> GetProduct(
        Guid id,
        [Service] IMediator mediator,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id), ct);
        return result.IsSuccess ? result.Value : null;
    }
}

