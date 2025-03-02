using CC.Auth.Api.Models.v1;

namespace CC.Auth.Api.Persistence.v1.Interfaces;

public interface IUserRepository
{
    Task<User?> TryGetUses(string email, string password);
}