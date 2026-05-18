using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Application.Interfaces.Services;
using NexoraBackend.Config;
using NexoraBackend.Infrastructure.Persistence;
using NexoraBackend.Infrastructure.Persistence.Repositories;
using NexoraBackend.Infrastructure.Services;
using StackExchange.Redis;


public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.SectionName));
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<RedisSettings>(configuration.GetSection(RedisSettings.SectionName));
        services.Configure<StripeSettings>(configuration.GetSection(StripeSettings.SectionName));
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));

        var dbSettings = configuration.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>()!;
        var redisSettings = configuration.GetSection(RedisSettings.SectionName).Get<RedisSettings>()!;

        // EF Core + PostgreSQL
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(dbSettings.ConnectionString, npgsql =>
            {
                npgsql.CommandTimeout(dbSettings.CommandTimeout);
                npgsql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
            });
            if (dbSettings.EnableSensitiveDataLogging) options.EnableSensitiveDataLogging();
            if (dbSettings.EnableDetailedErrors) options.EnableDetailedErrors();
        });

        // Redis
        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(redisSettings.ConnectionString));

        // Repositories (all scoped — one per HTTP request)
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();   
        services.AddScoped<ICouponRepository, CouponRepository>();    
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IPaymentService, StripePaymentService>();
        services.AddScoped<IEmailService, MailKitEmailService>();

        // Health Checks
        services.AddHealthChecks()
            .AddNpgSql(dbSettings.ConnectionString, name: "postgresql")
            .AddRedis(redisSettings.ConnectionString, name: "redis");

        return services;
    }
}