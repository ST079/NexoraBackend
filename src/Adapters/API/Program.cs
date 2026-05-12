
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NexoraBackend.API.Filters;
using NexoraBackend.API.GraphQL.Users;
using NexoraBackend.API.Middlewares;
using NexoraBackend.Config;
using NexoraBackend.Core.Domain.Ports;
using NexoraBackend.Infrastructure.Repositories;
using NexoraBackend.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Register infrastructure services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

//config
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddErrorFilter<GraphQLErrorFilter>()
    .ModifyRequestOptions(opt =>
    {
        opt.IncludeExceptionDetails = true;
    }).AddAuthorization();

//Adding the http concept to add the cookie
builder.Services.AddHttpContextAccessor();


//Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:issuer"],
        ValidAudience = builder.Configuration["Jwt:audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]!)
        )
    };
});

//build
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

//middlewares
app.UseMiddleware<AuditloggingMiddleware>();

app.MapGraphQL();

app.Run();

