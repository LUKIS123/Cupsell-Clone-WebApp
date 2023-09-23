using CupsellCloneAPI.Authentication.Models;
using CupsellCloneAPI.Database.Entities.User;

namespace CupsellCloneAPI.Authentication.Authenticators;

public interface IAuthenticator
{
    Task<AuthenticatedUserResponseDto> Authenticate(User user);
}