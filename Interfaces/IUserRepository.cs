using IsekaiFantasyBE.Models.Users;

namespace IsekaiFantasyBE.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserById(Guid id);

    Task<User?> GetUserByUsername(string username);

    Task<User?> GetUserByEmail(string email);

    Task<PreRegistrationUser?> GetPreRegisteredUserByEmail(string email);

    Task<PreRegistrationUser?> PreRegisterUser(PreRegistrationUser user);

    Task<User?> FinishRegisterUser(Guid token, string password);

    Task UpdateUserProperties(UserProperties newUserProperties);

    Task<BannedUsers> BanUser(User user, User bannedBy, DateTime bannedUntil, string reason);
}