using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using IsekaiFantasyBE.Database;

namespace IsekaiFantasyBE.Models.Users;

public class User
{
    [JsonIgnore]
    public Guid Id { get; set; }
    
    [Column(TypeName = DbProperties.Varchar32)] 
    public string Username { get; set; }
    
    [EmailAddress]
    public string Email { get; set; }
    
    public byte[] Password { get; set; }
    
    [JsonIgnore]
    public DateTime? CreatedAt { get; set; }
    
    [JsonIgnore]
    public DateTime? UpdatedAt { get; set; }
    
    [JsonIgnore]
    public DateTime? LastLogin { get; set; }
    
    [JsonIgnore]
    public ICollection<UserProperties>? Properties { get; set; }
}