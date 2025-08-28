using System.ComponentModel.DataAnnotations;
using IsekaiFantasyBE.Models.Users;

namespace IsekaiFantasyBE.Models.DTO;

public class Myself
{
    public Guid Userid { get; set; }

    public string? Username { get; set; }

    [EmailAddress] public string? Email { get; set; }

    public UserProperties? Properties { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastLogin { get; set; }
}