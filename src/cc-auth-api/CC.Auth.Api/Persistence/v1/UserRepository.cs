using CC.Auth.Api.Persistence.v1.Interfaces;

using Microsoft.Azure.Cosmos;

using User = CC.Auth.Api.Models.v1.User;

namespace CC.Auth.Api.Persistence.v1;

public class UserRepository(CosmosClient cosmosClient) : IUserRepository
{
    public Task<User?> TryGetUses(string email, string password)
    {
        var container = cosmosClient.GetContainer("core-coaching", User.ContainerId);
        // container.getitem
    }
}