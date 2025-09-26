namespace IsekaiFantasyBE.Models.Response.Entities;

public record SilencedResponse(
    UserResponse SilencedUser,
    UserResponse SilencedBy,
    DateTime SilencedAt,
    DateTime SilencedUntil,
    string Reason
);