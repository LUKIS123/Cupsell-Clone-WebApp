using CupsellCloneAPI.Authentication.Models;

namespace CupsellCloneAPI.Authentication.Services
{
    public interface IAccountService
    {
        Task RegisterUser(RegisterUserDto dto);
        Task<AuthenticatedUserResponseDto> LoginUser(LoginUserDto loginUserDto);
        Task<AuthenticatedUserResponseDto> RefreshUserJwt(string refreshToken);
        Task DeleteUserRefreshTokens();
        Task ResendUserVerificationEmail();
        Task VerifyUser(string encryptedToken);
    }
}