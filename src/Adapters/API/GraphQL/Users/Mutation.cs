using NexoraBackend.Application.DTOs.Inputs.Users;
using NexoraBackend.Application.DTOs.Responses.Auth;
using NexoraBackend.Application.DTOs.Responses.Users;
using NexoraBackend.Application.UseCases.Auth;
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

    public async Task<RegisterResponseDto> Register(RegisterDto registerDto, [Service] RegisterUserUseCase registerUseCase)
    {
        return await registerUseCase.Execute(registerDto);
    }

    public async Task<UserResponseDto> UpdateUser
    (UpdateUserDto updateUserDto, [Service] UpdateUserUseCase updateUserUseCase)
    {
        return await updateUserUseCase.Execute(updateUserDto);
    }
    public async Task<bool> DeleteUser(Guid id, [Service] DeleteUserUseCase deleteUserUseCase)
    {
        return await deleteUserUseCase.Execute(id);
    }

    public async Task<bool> Logout(string refreshToken, [Service] LogoutUseCase logoutUseCase)
    {
        return await logoutUseCase.Execute(refreshToken);
    }

    public async Task<AuthResponseDto> RefreshToken(string accessToken, string refreshToken, [Service] RefreshTokenUseCase refreshTokenUseCase)
    {
        return await refreshTokenUseCase.Execute(accessToken, refreshToken);
    }
}