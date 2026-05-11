
using NexoraBackend.Application.DTOs.Responses.Users;
using NexoraBackend.Application.Services;

namespace NexoraBackend.API.GraphQL.Users;

public class Query
{
    public async Task<IEnumerable<UserResponseDto>> GetUsers([Service] UserQueryService userService)
    {
       return await userService.GetUsersAsync();
    }


    public async Task<UserResponseDto?> GetUserById(Guid id, [Service] UserQueryService userService)
    {
        return await userService.GetUserByIdAsync(id);
    }

    public async Task<bool> GetUserByEmail(string email, [Service] UserQueryService userService)
    {
        return await userService.GetUserByEmail(email);
    }

    
}