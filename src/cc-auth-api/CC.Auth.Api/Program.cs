using System.Net;

using Asp.Versioning;

using Azure.Core;
using Azure.Identity;

using CC.Auth.Api.Extensions;
using CC.Auth.Api.Persistence.v1;
using CC.Auth.Api.Persistence.v1.Interfaces;

using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

TokenCredential credential = builder.Environment.IsDevelopment() ? new AzureCliCredential() : new ManagedIdentityCredential(builder.Configuration["ManagedIdentity:ClientId"]);

// ValidateAppSettings(builder.Configuration,
//     keysAndTypes:
//     [
//         (AppConfig.Keys.AppConfigConnectionString, typeof(string))
//     ]
// );
//
// builder.Configuration
//     .AddAzureAppConfiguration(options =>
//     {
//         var appConfigUri = new Uri(builder.Configuration[AppConfig.Keys.AppConfigConnectionString]!);
//         options
//             .Connect(appConfigUri, credential)
//             .Select(KeyFilter.Any, LabelFilter.Null)
//             .Select(KeyFilter.Any, "auth");
//     });

ValidateAppSettings(builder.Configuration,
    keysAndTypes:
    [
        (AppConfig.Keys.CosmosConnectionString, typeof(string)),
        (AppConfig.Keys.CosmosDatabaseId, typeof(string))
    ]
);

builder.Services
    .AddOpenApi()
    .AddScoped<IUserRepository, UserRepository>()
    .AddSingleton(_ => new CosmosClient(builder.Configuration[AppConfig.Keys.CosmosConnectionString]!, credential))
    .AddAzureClients(clientBuilder =>
    {
        clientBuilder.UseCredential(credential);
    });

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1);
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    });

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    var cosmosClient = app.Services.GetRequiredService<CosmosClient>();
    await InitializeCosmosDb(cosmosClient, builder.Configuration[AppConfig.Keys.CosmosDatabaseId]!);
}

app
    .UseHttpsRedirection()
    .UseAuthorization();

app.MapControllers();

app.Run();
return;

static async Task InitializeCosmosDb(CosmosClient cosmosClient, string databaseId)
{
    var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(
        id: databaseId,
        throughput: 400
    );

    switch (databaseResponse.StatusCode)
    {
        case HttpStatusCode.OK:
            return;
        case HttpStatusCode.Created:
            // TODO: Create container(s)?
            return;
        default:
            throw new ArgumentException($"Unexpected status code when creating Cosmos database. StatusCode: {databaseResponse.StatusCode}", nameof(databaseResponse.StatusCode));
    }
}

static void ValidateAppSettings(IConfiguration configuration, params (string Key, Type type)[] keysAndTypes)
{
    foreach (var (key, type) in keysAndTypes)
    {
        var value = configuration.GetValue(type, key, null);
        ArgumentNullException.ThrowIfNull(value);
    }
}