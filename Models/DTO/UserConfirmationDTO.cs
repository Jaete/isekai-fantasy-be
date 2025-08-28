using System.ComponentModel.DataAnnotations;

namespace IsekaiFantasyBE.Models.DTO;

public class UserConfirmationDTO
{
    public Guid Token { get; set; }
    public string Password { get; set; }
}