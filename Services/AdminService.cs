using IsekaiFantasyBE.Models.DTO;
using IsekaiFantasyBE.Models.Response;
using IsekaiFantasyBE.Models.Response.Entities;
using IsekaiFantasyBE.Models.Users;
using IsekaiFantasyBE.Repository;
using IsekaiFantasyBE.Services.Utils;

namespace IsekaiFantasyBE.Services;

public class AdminService
{
    private readonly UserRepository _userRepository;
    public AdminService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ResponseModel> UpdateUserProperties (Guid userId, UserPropertiesDTO newProperties)
    {
        var user = await _userRepository.GetUserById(userId);
        if (user is null)
        {
            return ResponseService.NotFound(ApiMessages.UserNotFound);
        }

        await _userRepository.UpdateUserProperties(
            new UserProperties
            {
                User = user,
                Photo = newProperties.Photo,
                Bio = newProperties.Bio,
                LastActivity = DateTime.Now,
            }
        );
        
        return ResponseService.Ok(
            new UserResponse(
                user.Id, user.Username, user.Properties, user.CreatedAt, user.UpdatedAt
            ),
            ApiMessages.UserUpdated
        );
    }

    public async Task<ResponseModel> BanUser(Guid userId, BanUserDTO banUserProps, HttpContext httpContext)
    {
        var admin = await _userRepository.GetUserById(JwtService.GetAuthenticatedUserId(httpContext));
        var user = await _userRepository.GetUserById(userId);
        if (user is null)
        {
            return ResponseService.NotFound(ApiMessages.UserNotFound);
        }

        var bannedUser = await _userRepository.BanUser(
            user,
            admin!,
            banUserProps.BannedUntil,
            banUserProps.Reason
        );

        return ResponseService.Ok(
            new BannedResponse(
                BannedUser: new UserResponse(
                    Id: bannedUser.User.Id,
                    Username: bannedUser.User.Username
                ),
                BannedBy: new UserResponse(
                    Id: bannedUser.BannedBy.Id,
                    Username: bannedUser.BannedBy.Username
                ),
                Reason: banUserProps.Reason,
                BannedAt: bannedUser.BannedAt,
                BannedUntil: banUserProps.BannedUntil
            ),
            ApiMessages.UserBanned
        );
    }
}