using IsekaiFantasyBE.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace IsekaiFantasyBE.Contexts;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options): base(options){}
    
    public DbSet<User> Users { get; set; }
    public DbSet<UserProperties> UsersProperties { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<User>()
            .Property(u => u.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();

        modelBuilder.Entity<User>()
            .Property(u => u.LastLogin)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<UserProperties>()
            .Property(up => up.LastActivity)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();
    }
}