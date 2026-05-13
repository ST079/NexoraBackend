


using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Authorization;
using NexoraBackend.Application.DTOs.Responses.Users;
using NexoraBackend.Application.Services;

namespace NexoraBackend.API.GraphQL.Users;

public class Query
{
    [Authorize]
    public async Task<IEnumerable<UserResponseDto>> GetUsers([Service] UserQueryService userService)
    {
        return await userService.GetUsersAsync();
    }


    [Authorize]
    public async Task<UserResponseDto?> GetUserById(Guid id, [Service] UserQueryService userService)
    {
        return await userService.GetUserByIdAsync(id);
    }

    public async Task<bool> GetUserByEmail(string email, [Service] UserQueryService userService)
    {
        return await userService.GetUserByEmail(email);
    }


}