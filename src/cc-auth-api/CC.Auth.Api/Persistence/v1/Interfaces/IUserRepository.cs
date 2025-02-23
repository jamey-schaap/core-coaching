using CC.Auth.Api.Models.v1;

namespace CC.Auth.Api.Persistence.v1.Interfaces;

public interface IUserRepository
{
    Task<User> GetUser(string email, string password);
}