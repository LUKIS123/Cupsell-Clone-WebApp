using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CupsellCloneAPI.Authentication.Models;
using CupsellCloneAPI.Authentication.Settings;
using CupsellCloneAPI.Database.Entities.User;
using Microsoft.IdentityModel.Tokens;

namespace CupsellCloneAPI.Authentication.TokenGenerators
{
    public class DefaultTokenGenerator : ITokenGenerator
    {
        private readonly AuthenticationSettings _authenticationSettings;

        public DefaultTokenGenerator(AuthenticationSettings authenticationSettings)
        {
            _authenticationSettings = authenticationSettings;
        }

        public AccessTokenDto GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, $"{user.Name} {user.LastName}"),
                new(ClaimTypes.Role, user.Role.Name),
                new(ClaimTypes.Email, user.Email),
                new("Username", user.Username)
            };
            if (user.DateOfBirth.HasValue)
            {
                claims.Add(new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd")));
            }

            var expirationTime = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtExpireMinutes);
            var value = GenerateJwt(
                _authenticationSettings.JwtKey,
                _authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtAudience,
                expirationTime,
                claims
            );

            return new AccessTokenDto()
            {
                Value = value,
                ExpirationTime = expirationTime
            };
        }

        public string GenerateRefreshToken()
        {
            var expirationTime = DateTime.UtcNow.AddMinutes(_authenticationSettings.RefreshTokenExpireMinutes);

            return GenerateJwt(
                _authenticationSettings.RefreshTokenKey,
                _authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtAudience,
                expirationTime
            );
        }

        public static string GenerateJwt(string secretKey, string issuer, string audience, DateTime expirationTime,
            IEnumerable<Claim>? claims = null)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: expirationTime,
                signingCredentials: cred
            );
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}