using IsekaiFantasyBE.Database;
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
            .HasDefaultValueSql(DbProperties.CurrentTimestamp)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<User>()
            .Property(u => u.UpdatedAt)
            .HasDefaultValueSql(DbProperties.CurrentTimestamp)
            .ValueGeneratedOnAddOrUpdate();

        modelBuilder.Entity<User>()
            .Property(u => u.LastLogin)
            .HasDefaultValueSql(DbProperties.CurrentTimestamp)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<UserProperties>()
            .Property(up => up.LastActivity)
            .HasDefaultValueSql(DbProperties.CurrentTimestamp)
            .ValueGeneratedOnAdd();
    }
}