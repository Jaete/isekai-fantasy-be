using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using IsekaiFantasyBE.Database;

namespace IsekaiFantasyBE.Models.Users;

public class UserProperties
{
    public const string BANNED = "banned";
    public const string ACTIVE = "active";
    public enum Role { Admin = 0, Member, Moderation, Narrator, }
    
    [JsonIgnore]
    public int Id { get; set; }
    
    [JsonIgnore][ForeignKey("UserId")]
    public User User { get; set; }
    
    [Column(TypeName = DbProperties.Varchar255)]
    public string? Photo { get; set; }
    
    [Column(TypeName = DbProperties.Text)]
    public string? Bio { get; set; }
    
    [Column(TypeName = DbProperties.Varchar32)]
    public string? Status { get; set; }
    
    public Role? UserRole { get; set; }
    
    public DateTime? LastActivity { get; set; }
}