// using HotChocolate.Authorization;
// using MediatR;
// using NexoraBackend.API.GraphQL.Types;

// namespace NexoraBackend.API.GraphQL.Mutations;

// [ExtendObjectType(OperationTypeNames.Mutation)]
// public class CategoryMutations
// {
//     [Authorize(Roles = new[] { "Admin", "SuperAdmin" })]
//     public async Task<CategoryPayload> CreateCategory(
//         CreateCategoryInput input, [Service] IMediator mediator, CancellationToken ct = default)
//     {
//         var result = await mediator.Send(new CreateCategoryCommand(
//             input.Name, input.Description, input.ParentId, input.ImageUrl, input.SortOrder), ct);
//         return result.Match(
//             onSuccess: dto => new CategoryPayload { Data = dto },
//             onFailure: err => new CategoryPayload { Error = err.Message });
//     }
// }