using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NexoraBackend.Application.Mappings;
using NexoraBackend.Application.Services;
using NexoraBackend.Application.UseCases.Auth;
using NexoraBackend.Application.UseCases.Users;
using NexoraBackend.Application.Validators.Users;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //usecases
        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<UpdateUserUseCase>();
        services.AddScoped<DeleteUserUseCase>();
        services.AddScoped<RegisterUserUseCase>();

        //mappers
        services.AddScoped<UserMapper>();

        //validators
        services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();

        //services
        services.AddScoped<UserQueryService>();

        return services;
    }
}