using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using IsekaiFantasyBE.Database;

namespace IsekaiFantasyBE.Models.Users;

public class SilencedUsers
{
    [JsonIgnore]
    public int Id { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; }
    
    [JsonIgnore][Column(TypeName = DbProperties.Datetime)][DefaultValue(DbProperties.CurrentTimestamp)] 
    public DateTime SilencedAt { get; set; }
    
    [JsonIgnore][Column(TypeName = DbProperties.Datetime)][DefaultValue(DbProperties.CurrentTimestamp)] 
    public DateTime SilencedUntil { get; set; }
    
    [ForeignKey("SilencedById")]
    public User SilencedBy  { get; set; }
    
    public string? Reason { get; set; }
}