
using NexoraBackend.API.GraphQL.Users;
using NexoraBackend.Config;
using NexoraBackend.Core.Domain.Ports;
using NexoraBackend.Infrastructure.Repositories;
using NexoraBackend.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Register infrastructure services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, JwtService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

//config
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .ModifyRequestOptions(opt =>
    {
        opt.IncludeExceptionDetails = true;
    });

var app = builder.Build();

app.MapGraphQL();

app.Run();

