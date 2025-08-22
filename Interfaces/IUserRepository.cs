using IsekaiFantasyBE.Models.Users;

namespace IsekaiFantasyBE.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserById(Guid id);

    Task<User?> GetUserByUsername(string username);

    Task<User?> GetUserByEmail(string email);

    Task<User?> RegisterNewUser(User user);
}