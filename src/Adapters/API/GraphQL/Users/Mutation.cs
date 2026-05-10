using System.Net.Security;
using NexoraBackend.Application.DTOs.Inputs.Users;
using NexoraBackend.Application.DTOs.Responses;
using NexoraBackend.Application.DTOs.Responses.Users;
using NexoraBackend.Application.UseCases.Users;
namespace NexoraBackend.API.GraphQL.Users;

public class Mutation
{
    public async Task<UserResponseDto> CreateUser
    (CreateUserDto createUserDto, [Service] CreateUserUseCase createUserUseCase)
    {
        return await createUserUseCase.Execute(createUserDto);
    }

    public async Task<LoginResponseDto> Login(LoginDto loginDto, [Service] LoginUseCase loginUseCase)
    {
        return await loginUseCase.Execute(loginDto);
    }

    // public async Task<User> UpdateUser
    // (UpdateUserInput input, [Service] UpdateUserUseCase updateUserUseCase)
    // {
    //     User updatedUser = new User { Id = input.Id, Name = input.Name!, Email = input.Email!, Phone = input.Phone!, Address = input.Address! };
    //     return await updateUserUseCase.Execute(input.Id, input);
    // }

    public async Task<bool> DeleteUser(Guid id, [Service] DeleteUserUseCase deleteUserUseCase)
    {
        return await deleteUserUseCase.Execute(id);
    }
}