using CupsellCloneAPI.Authentication.Models;
using CupsellCloneAPI.Database.Entities.User;

namespace CupsellCloneAPI.Authentication.TokenGenerators;

public interface ITokenGenerator
{
    AccessTokenDto GenerateAccessToken(User user);
    string GenerateRefreshToken();
}