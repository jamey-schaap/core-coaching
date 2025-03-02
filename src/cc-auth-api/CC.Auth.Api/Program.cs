using System.Net;
using System.Reflection;

using Asp.Versioning;

using Azure.Core;
using Azure.Identity;

using CC.Auth.Api.Constants.v1;
using CC.Auth.Api.Persistence.v1;
using CC.Auth.Api.Persistence.v1.Interfaces;
using CC.Common.Configuration;

using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

TokenCredential credential = builder.Environment.IsDevelopment() ? new AzureCliCredential() : new ManagedIdentityCredential(builder.Configuration["ManagedIdentity:ClientId"]);

if (!builder.Environment.IsDevelopment())
{
    builder.Configuration
        .ValidateKeys((AppConfiguration.Keys.AppConfigConnectionString, typeof(string)))
        .AddAzureAppConfiguration(options =>
        {
            var appConfigUri = new Uri(builder.Configuration[AppConfiguration.Keys.AppConfigConnectionString]!);
            options
                .Connect(appConfigUri, credential)
                .Select(KeyFilter.Any, LabelFilter.Null)
                .Select(KeyFilter.Any, "auth");
        });
}


builder.Configuration.ValidateAllKeys(typeof(AppConfiguration.Keys));

builder.Services
    .AddOpenApi()
    .AddScoped<IUserRepository, UserRepository>()
    .AddSingleton(_ => new CosmosClient(builder.Configuration[AppConfiguration.Keys.CosmosConnectionString]!, credential))
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
}

app
    .UseHttpsRedirection()
    .UseAuthorization();

app.MapControllers();

app.Run();