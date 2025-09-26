using System.ComponentModel.DataAnnotations;

namespace IsekaiFantasyBE.Models.DTO;

public record UserDTO(
    string? Username,
    string? Email,
    string? Password
);