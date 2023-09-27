using CupsellCloneAPI.Authentication.Models;
using CupsellCloneAPI.Authentication.TokenGenerators;
using CupsellCloneAPI.Database.Authentication.Repositories;
using CupsellCloneAPI.Database.Entities.User;

namespace CupsellCloneAPI.Authentication.Authenticators
{
    public class Authenticator : IAuthenticator
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public Authenticator(ITokenGenerator tokenGenerator, IRefreshTokenRepository refreshTokenRepository)
        {
            _tokenGenerator = tokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<AuthenticatedUserResponseDto> Authenticate(User user)
        {
            var accessToken = _tokenGenerator.GenerateAccessToken(user);
            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            await _refreshTokenRepository.Create(user.Id, refreshToken);

            return new AuthenticatedUserResponseDto()
            {
                AccessToken = accessToken.Value,
                AccessTokenExpirationTime = accessToken.ExpirationTime,
                RefreshToken = refreshToken
            };
        }
    }
}