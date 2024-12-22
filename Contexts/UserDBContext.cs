using IsekaiFantasyBE.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace IsekaiFantasyBE.Contexts;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options): base(options){}
    
    public DbSet<User> Users { get; set; }
    public DbSet<UserProperties> UsersProperties { get; set; }
}