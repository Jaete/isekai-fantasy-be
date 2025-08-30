using IsekaiFantasyBE.Database;
using IsekaiFantasyBE.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace IsekaiFantasyBE.Contexts;

public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options): base(options){}
    
    public DbSet<User> Users { get; set; }
    public DbSet<UserProperties> UsersProperties { get; set; }
    public DbSet<PreRegistrationUser> PreRegistrationUsers { get; set; }
    
    public DbSet<BannedUsers> BannedUsers { get; set; }
    
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

        modelBuilder.Entity<PreRegistrationUser>()
           .Property(u => u.CreatedAt)
           .HasDefaultValueSql(DbProperties.CurrentTimestamp)
           .ValueGeneratedOnAdd();

        modelBuilder.Entity<PreRegistrationUser>()
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

        modelBuilder.Entity<UserProperties>()
            .Property(up => up.UserRole)
            .HasDefaultValue(UserProperties.Role.Member)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<BannedUsers>()
            .Property(bu => bu.BannedAt)
            .HasDefaultValueSql(DbProperties.CurrentTimestamp)
            .ValueGeneratedOnAdd();
    }
}