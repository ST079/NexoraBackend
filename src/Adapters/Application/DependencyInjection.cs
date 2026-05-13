using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NexoraBackend.Application.Mappings;
using NexoraBackend.Application.Services;
using NexoraBackend.Application.UseCases.Auth;
using NexoraBackend.Application.UseCases.Roles;
using NexoraBackend.Application.UseCases.Users;
using NexoraBackend.Application.Validators.Users;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //usecases
        //user
        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<UpdateUserUseCase>();
        services.AddScoped<DeleteUserUseCase>();
        services.AddScoped<RegisterUserUseCase>();
        services.AddScoped<AddRoleUseCase>();
        services.AddScoped<LogoutUseCase>();
        services.AddScoped<AssignRoleUseCase>();
        services.AddScoped<DeleteRoleUseCase>();

        //roles
        services.AddScoped<AddRoleUseCase>();

        //mappers
        services.AddScoped<UserMapper>();
        services.AddScoped<RefreshTokenMapper>();
        services.AddScoped<AuditLogMapper>();
        services.AddScoped<ProductMapper>();
        services.AddScoped<RoleMapper>();



        //validators
        services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();

        //services
        services.AddScoped<UserQueryService>();
        services.AddScoped<RoleQueryService>();

        return services;
    }
}