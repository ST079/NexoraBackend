using Serilog;
using HotChocolate.Language;
using NexoraBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
// using NexoraBackend.API.Endpoints;
using NexoraBackend.API.Middlewares;
using NexoraBackend.API.Filters;
using StackExchange.Redis;
using NexoraBackend.API.GraphQL.Subscriptions;
using NexoraBackend.Adapters.API.GraphQL.Types;
using NexoraBackend.API.GraphQL.Mutations;
using NexoraBackend.API.GraphQL.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NexoraBackend.Config;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// === Serilog ===
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// === Layers ===
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// === HTTP ===
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// === Rate Limiting ===
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("global", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100, Window = TimeSpan.FromMinutes(1), QueueLimit = 10
            }));

    options.AddPolicy("auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5, Window = TimeSpan.FromMinutes(1)
            }));
});

// === JWT Auth ===
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ValidateIssuer   = true, ValidIssuer   = jwtSettings.Issuer,
            ValidateAudience = true, ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew        = TimeSpan.Zero
        };

        // Support WebSocket tokens for GraphQL subscriptions
        opt.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var token = ctx.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(token)) ctx.Token = token;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// === GraphQL (Hot Chocolate) ===
// Each query/mutation file uses [ExtendObjectType(OperationTypeNames.*)]
// and is registered here with AddTypeExtension.
builder.Services
    .AddGraphQLServer()
    // Root operation types (empty shells extended by the *Queries/*Mutations classes)
    .AddQueryType(d => d.Name(OperationTypeNames.Query))
    .AddMutationType(d => d.Name(OperationTypeNames.Mutation))
    .AddSubscriptionType(d => d.Name(OperationTypeNames.Subscription))
    // Queries
    .AddTypeExtension<ProductQueries>()
    .AddTypeExtension<OrderQueries>()
    .AddTypeExtension<UserQueries>()
    // .AddTypeExtension<CartQueries>()
    // .AddTypeExtension<CategoryQueries>()
    // .AddTypeExtension<ReviewQueries>()
    .AddTypeExtension<CouponQueries>()
    // Mutations
    .AddTypeExtension<AuthMutations>()
    .AddTypeExtension<CartMutations>()
    .AddTypeExtension<OrderMutations>()
    .AddTypeExtension<ProductMutations>()
    .AddTypeExtension<ReviewMutations>()
    .AddTypeExtension<CouponMutations>()
    // .AddTypeExtension<CategoryMutations>()
    // Subscriptions
    .AddTypeExtension<OrderSubscriptions>()
    // Object types
    .AddType<ProductType>()
    // .AddType<OrderType>()
    // .AddType<UserType>()
    // Features
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .AddAuthorization()
    .AddRedisSubscriptions(provider =>
        provider.GetRequiredService<IConnectionMultiplexer>())
    .AddErrorFilter<GraphQLErrorFilter>()
    .ModifyRequestOptions(opt =>
    {
        opt.IncludeExceptionDetails = builder.Environment.IsDevelopment();
    });

var app = builder.Build();

// === Middleware pipeline ===
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();
app.UseCors("AllowAll");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();
app.MapGraphQL("/graphql");
app.MapHealthChecks("/health");
// app.MapStripeWebhook();   // POST /webhooks/stripe

// === Auto-migrate on startup (dev only) ===
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

await app.RunAsync();

