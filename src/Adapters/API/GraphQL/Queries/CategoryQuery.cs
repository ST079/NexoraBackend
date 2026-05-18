using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Queries.Categories;

namespace NexoraBackend.API.GraphQL.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public class CategoryQueries
{
    public async Task<IEnumerable<CategoryDto>> GetCategories(
        [Service] IMediator mediator,
        bool rootOnly = true,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetCategoriesQuery(rootOnly), ct);
        return result.IsSuccess ? result.Value! : [];
    }
}