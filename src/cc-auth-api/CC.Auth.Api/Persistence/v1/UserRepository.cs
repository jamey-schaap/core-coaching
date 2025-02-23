using CC.Auth.Api.Persistence.v1.Interfaces;

using Microsoft.Azure.Cosmos;

using User = CC.Auth.Api.Models.v1.User;

namespace CC.Auth.Api.Persistence.v1;

public class UserRepository(CosmosClient cosmosClient) : IUserRepository
{
    public Task<User> GetUser(string email, string password)
    {
        // cosmosClient.GetContainer("core-coaching");
        throw new NotImplementedException();
    }
}