namespace IsekaiFantasyBE.Models.Response.Entities;

public record BannedResponse(
    UserResponse BannedUser,
    UserResponse BannedBy,
    DateTime BannedAt,
    DateTime BannedUntil,
    string Reason
);