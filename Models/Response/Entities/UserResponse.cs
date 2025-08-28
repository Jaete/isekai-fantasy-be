using IsekaiFantasyBE.Models.Users;

namespace IsekaiFantasyBE.Models.Response.Entities;

public record UserResponse(
    Guid Id,
    string Username,
    UserProperties? Properties = null,
    DateTime? Created = null,
    DateTime? Updated = null,
    DateTime? LastLogin = null
);