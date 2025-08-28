using System.ComponentModel.DataAnnotations;
using IsekaiFantasyBE.Models.Users;

namespace IsekaiFantasyBE.Models.DTO;

public record Myself (
    Guid Userid, 
    string Username,
    [EmailAddress] string? Email,
    UserProperties? Properties,
    DateTime? CreatedAt,
    DateTime? UpdatedAt,
    DateTime? LastLogin
);