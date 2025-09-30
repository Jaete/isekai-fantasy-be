using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using IsekaiFantasyBE.Services.Utils;

namespace IsekaiFantasyBE.Models.Users.Requests;

public class PasswordResetRequest
{
    [JsonIgnore]
    public Guid Id;
    
    [JsonIgnore][ForeignKey("UserId")]
    public User User { get; set; }
    
    public Guid ResetToken { get; set; }
    
    public PasswordResetStatus Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime Expires { get; set; }

    public static PasswordResetRequest Create(User user)
    {
        return new PasswordResetRequest
        {
            User = user,
            ResetToken = Credentials.GenerateValidationToken(),
            CreatedAt = DateTime.Now,
            Expires = DateTime.Now.AddMinutes(10),
            Status = PasswordResetStatus.Pending
        };
    }
}