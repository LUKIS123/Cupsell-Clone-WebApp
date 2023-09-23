using CupsellCloneAPI.Authentication.Models;
using CupsellCloneAPI.Database.Entities.User;
using System.Security.Claims;

namespace CupsellCloneAPI.Authentication.TokenGenerators;

public interface ITokenGenerator
{
    AccessTokenDto GenerateAccessToken(User user);

    string GenerateRefreshToken();

    string GenerateJwt(string secretKey, string issuer, string audience, DateTime expirationTime,
        IEnumerable<Claim>? claims = null);
}