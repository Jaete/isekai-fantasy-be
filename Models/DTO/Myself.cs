using System.ComponentModel.DataAnnotations;
using IsekaiFantasyBE.Models.Users;

namespace IsekaiFantasyBE.Models.DTO;

public record Myself(
    Guid Userid,
    string Username,
    [EmailAddress] string? Email,
    UserProperties? Properties,
    DateTime? CreatedAt,
    DateTime? UpdatedAt,
    DateTime? LastLogin
)
{
    public Myself(User user) : this
    (
        user.Id,
        user.Username,
        user.Email,
        user.Properties is not null
            ? new UserProperties
            {
                Bio = user.Properties.Bio,
                Photo = user.Properties.Photo,
                LastActivity = user.Properties.LastActivity,
                Status = user.Properties.Status,
                UserRole = user.Properties.UserRole
            }
            : null,
        user.CreatedAt,
        user.UpdatedAt,
        user.LastLogin)
    {}
};