using HotChocolate.Authorization;
using NexoraBackend.Application.DTOs.Inputs.Users;
using NexoraBackend.Application.DTOs.Responses.Auth;
using NexoraBackend.Application.DTOs.Responses.Users;
using NexoraBackend.Application.UseCases.Auth;
using NexoraBackend.Application.UseCases.Users;
namespace NexoraBackend.API.GraphQL.Users;

public class Mutation
{
    [Authorize]
    public async Task<UserResponseDto> CreateUser
   (CreateUserDto createUserDto, [Service] CreateUserUseCase createUserUseCase)
    {
        return await createUserUseCase.Execute(createUserDto);
    }

    public async Task<LoginResponseDto> Login(LoginDto loginDto, [Service] LoginUseCase loginUseCase, [Service] IHttpContextAccessor httpContextAccessor)
    {
        var result = await loginUseCase.Execute(loginDto);
        var response = httpContextAccessor.HttpContext!.Response;

        response.Cookies.Append("accessToken", result.Token.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMinutes(15)
        });

        response.Cookies.Append("refreshToken", result.Token.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, //true for https and false for http
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(2)
        });

        return result;
    }

    public async Task<RegisterResponseDto> Register(RegisterDto registerDto, [Service] RegisterUserUseCase registerUseCase)
    {
        return await registerUseCase.Execute(registerDto);
    }

    [Authorize]
    public async Task<UserResponseDto> UpdateUser
   (UpdateUserDto updateUserDto, [Service] UpdateUserUseCase updateUserUseCase)
    {
        return await updateUserUseCase.Execute(updateUserDto);
    }

    [Authorize]
    public async Task<bool> DeleteUser(Guid id, [Service] DeleteUserUseCase deleteUserUseCase)
    {
        return await deleteUserUseCase.Execute(id);
    }


    [Authorize]
    public async Task<bool> Logout(string refreshToken, [Service] LogoutUseCase logoutUseCase, [Service] IHttpContextAccessor httpContextAccessor)
    {
        await logoutUseCase.Execute(refreshToken);
        var response = httpContextAccessor.HttpContext!.Response;
        response.Cookies.Delete("accessToken");
        response.Cookies.Delete("refreshToken");
        return true;
    }

    public async Task<AuthResponseDto> RefreshToken(string accessToken, string refreshToken, [Service] RefreshTokenUseCase refreshTokenUseCase)
    {
        return await refreshTokenUseCase.Execute(accessToken, refreshToken);
    }
}