
using NexoraBackend.Adapters.API.GraphQL.Queries;

var builder = WebApplication.CreateBuilder(args);

// Register infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .ModifyRequestOptions(opt =>
    {
        opt.IncludeExceptionDetails = true;
    });

var app = builder.Build();

app.MapGraphQL();

app.Run();

