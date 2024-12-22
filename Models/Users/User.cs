using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IsekaiFantasyBE.Database;

namespace IsekaiFantasyBE.Models.Users;

public class User
{
    public Guid Id { get; set; }
    
    [Column(TypeName = DbProperties.Varchar32)] 
    public string Username { get; set; }
    
    [EmailAddress] 
    public string Email { get; set; }
    
    public byte[] Password { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public DateTime LastLogin { get; set; }
    
    public ICollection<UserProperties> Properties { get; set; }
}