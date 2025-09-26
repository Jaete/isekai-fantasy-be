namespace IsekaiFantasyBE.Models.DTO;

public record UserConfirmationDTO
(
    Guid Token,
    string Password
);