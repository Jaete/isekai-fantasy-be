using IsekaiFantasyBE.Contexts;
using IsekaiFantasyBE.Interfaces;
using IsekaiFantasyBE.Models.Users;
using IsekaiFantasyBE.Services.Utils;
using Microsoft.EntityFrameworkCore;

namespace IsekaiFantasyBE.Repository;

public class UserRepository : IUserRepository
{
    private AppDBContext _dbContext;
    
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
        return await _dbContext.Users
        .Include(u => u.Properties)
        .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<PreRegistrationUser?> PreRegisterUser(PreRegistrationUser user)
    {
        try
        {
            _dbContext.PreRegistrationUsers.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<User?> FinishRegisterUser(Guid token, string password)
    {
        try
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.UsersProperties.AddAsync(
                new UserProperties
                {
                    User = user,
                    Status = UserProperties.ACTIVE,
                }
            );
            PreRegistrationUser? preRegister = await _dbContext.PreRegistrationUsers.FirstOrDefaultAsync<PreRegistrationUser>(
                preReg => preReg.EmailValidationToken == token
            );
            if (preRegister is null)
            {
                return null;
            }
            
            var encryptedPass = Encryption.Encrypt(password);
            if (!Encryption.Compare(encryptedPass, preRegister.Password))
            { 
                return null;
            }
            
            User user = new User()
            {
                Id = preRegister.Id,
                Email = preRegister.Email,
                Username = preRegister.Username,
                Password = preRegister.Password,
            };
            
            var properties = new UserProperties
            {
                User = user,
                Status = UserProperties.ACTIVE,
            };
            user.Properties = properties;
            
            await _dbContext.Users.AddAsync(user);
            await _dbContext.UsersProperties.AddAsync(properties);
            _dbContext.PreRegistrationUsers.Remove(preRegister);
            await _dbContext.SaveChangesAsync();

            return user;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task UpdateUserProperties(UserProperties newUserProperties)
    {
        var properties = await _dbContext.UsersProperties.FirstOrDefaultAsync(up => up.User.Id == newUserProperties.User.Id);
        if (properties is null)
        {
            throw new Exception("User properties not found");
        }
        
        properties.Bio = newUserProperties.Bio ?? properties.Bio;
        properties.Photo = newUserProperties.Photo ?? properties.Photo;
        properties.LastActivity = DateTime.Now;

        _dbContext.UsersProperties.Update(properties);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<BannedUsers> BanUser(User user, User bannedBy, DateTime bannedUntil, string reason)
    {
        var bannedUser = new BannedUsers()
        {
            User = user,
            BannedBy = bannedBy,
            BannedUntil = bannedUntil,
            BannedAt = DateTime.Now,
            Reason = reason
        };
        
        _dbContext.BannedUsers.Add(bannedUser);
        
        await _dbContext.SaveChangesAsync();
        return bannedUser;
    }

    public async Task<PreRegistrationUser> GetPreRegisteredUserByEmail(string email)
    {
        return await _dbContext.PreRegistrationUsers.FirstOrDefaultAsync(up => up.Email == email);
    }
}