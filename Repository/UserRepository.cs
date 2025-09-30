using IsekaiFantasyBE.Contexts;
using IsekaiFantasyBE.Interfaces;
using IsekaiFantasyBE.Models.DTO;
using IsekaiFantasyBE.Models.Response;
using IsekaiFantasyBE.Models.Response.Entities;
using IsekaiFantasyBE.Models.Users;
using IsekaiFantasyBE.Models.Users.Requests;
using IsekaiFantasyBE.Services.Utils;
using Microsoft.EntityFrameworkCore;

namespace IsekaiFantasyBE.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDBContext _dbContext;
    
    public UserRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetUserById(Guid id)
    {
        return await _dbContext.Users
            .Include(u => u.Properties)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        return await _dbContext.Users
            .Include(u => u.Properties)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _dbContext.Users
            .Include(u => u.Properties)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<PreRegistrationUser?> PreRegisterUser(PreRegistrationUser user)
    {
        _dbContext.PreRegistrationUsers.Add(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<PreRegistrationUser?> GetPreRegisteredUserByEmail(string email)
    {
        return await _dbContext.PreRegistrationUsers.FirstOrDefaultAsync(up => up.Email == email);
    }

    public async Task<PreRegistrationUser?> GetPreRegisteredUserByToken(Guid token)
    {
        return await _dbContext.PreRegistrationUsers.FirstOrDefaultAsync(
            preReg => preReg.EmailValidationToken == token
        );
    }
    
    public async Task<User?> FinishRegisterUser(PreRegistrationUser preRegister)
    {
        var user = new User
        {
            Id = preRegister.Id,
            Email = preRegister.Email,
            Username = preRegister.Username,
            Password = preRegister.Password,
        };
        
        var properties = new UserProperties
        {
            User = user,
            Status = UserStatus.Active,
        };
        user.Properties = properties;
        
        await _dbContext.Users.AddAsync(user);
        await _dbContext.UsersProperties.AddAsync(properties);
        _dbContext.PreRegistrationUsers.Remove(preRegister);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task UpdateUserProperties(UserProperties newProperties)
    {
        var properties = await _dbContext.UsersProperties.FirstOrDefaultAsync(up => up.User.Id == newProperties.User.Id);
        if (properties is null)
        {
            throw new KeyNotFoundException(ApiMessages.PropertiesNotFound);
        }
        
        properties.Bio = newProperties.Bio ?? properties.Bio;
        properties.Photo = newProperties.Photo ?? properties.Photo;
        properties.LastActivity = DateTime.Now;

        _dbContext.UsersProperties.Update(properties);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<BannedUsers> BanUser(User user, User bannedBy, DateTime bannedUntil, string reason)
    {
        var bannedUser = new BannedUsers
        {
            User = user,
            BannedBy = bannedBy,
            BannedUntil = bannedUntil,
            BannedAt = DateTime.Now,
            Reason = reason
        };

        user.Properties.Status = UserStatus.Banned;
        _dbContext.UsersProperties.Update(user.Properties);
        _dbContext.BannedUsers.Add(bannedUser);
        
        await _dbContext.SaveChangesAsync();
        return bannedUser;
    }


    public async Task<SilencedUsers> SilenceUser(User user, User admin, SilenceUserDTO silenceUserProps)
    {
        user.Properties.Status = UserStatus.Silenced;
        var silencedUser = new SilencedUsers
        {
            User = user,
            SilencedBy = admin,
            SilencedAt = DateTime.Now,
            SilencedUntil = silenceUserProps.SilencedUntil,
            Reason = silenceUserProps.Reason
        };

        _dbContext.SilencedUsers.Add(silencedUser);
        _dbContext.UsersProperties.Update(user.Properties);
        await _dbContext.SaveChangesAsync();
        return silencedUser;
    }

    public async Task<PasswordResetRequest> NewPasswordResetRequest(User user)
    {
        var request = PasswordResetRequest.Create(user);
        _dbContext.PasswordResetRequests.Add(request);
        await _dbContext.SaveChangesAsync();
        return request;
    }
}