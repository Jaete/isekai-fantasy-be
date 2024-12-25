using IsekaiFantasyBE.Contexts;
using IsekaiFantasyBE.Interfaces;
using IsekaiFantasyBE.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace IsekaiFantasyBE.Repository;

public class UserRepository : IUserRepository
{
    private UserDbContext _dbContext;
    
    public UserRepository(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetUserById(Guid id)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> RegisterNewUser(User user)
    {
        try
        {
            await _dbContext.Users.AddAsync(user);

            var properties = new UserProperties
            {
                User = user,
                Status = UserProperties.ACTIVE,
            };

            await _dbContext.UsersProperties.AddAsync(properties);
            await _dbContext.SaveChangesAsync();

            return user;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}