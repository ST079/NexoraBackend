using HotChocolate.Authorization;
using MediatR;
using NexoraBackend.API.GraphQL.Types;
using NexoraBackend.Application.Commands.Products;

namespace NexoraBackend.API.GraphQL.Mutations;

[MutationType]
public class ProductMutations
{
    [Authorize(Roles = new[] { "Admin", "SuperAdmin" })]
    public async Task<ProductPayload> CreateProduct(
        CreateProductInput input,
        [Service] IMediator mediator,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new CreateProductCommand(
            input.Name, input.Description, input.Price, input.Currency,
            input.StockQuantity, input.CategoryId, input.IsFeatured, input.ImageUrls), ct);

        return result.Match(
            onSuccess: dto => new ProductPayload { Data = dto },
            onFailure: err => new ProductPayload { Error = err.Message });
    }

    [Authorize(Roles = new[] { "Admin", "SuperAdmin" })]
    public async Task<ProductPayload> UpdateProductStock(
        Guid productId, int quantity,
        [Service] IMediator mediator,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new UpdateStockCommand(productId, quantity), ct);
        return result.Match(
            onSuccess: dto => new ProductPayload { Data = dto },
            onFailure: err => new ProductPayload { Error = err.Message });
    }
}
