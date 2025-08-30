using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using IsekaiFantasyBE.Database;

namespace IsekaiFantasyBE.Models.Users;

public class BannedUsers
{
    [JsonIgnore]
    public int Id { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; }
    
    [JsonIgnore][Column(TypeName = DbProperties.Datetime)][DefaultValue(DbProperties.CurrentTimestamp)] 
    public DateTime BannedAt { get; set; }
    
    [JsonIgnore][Column(TypeName = DbProperties.Datetime)][DefaultValue(DbProperties.CurrentTimestamp)] 
    public DateTime BannedUntil { get; set; }
    
    [ForeignKey("BannedById")]
    public User BannedBy  { get; set; }
    
    public string? Reason { get; set; }
}