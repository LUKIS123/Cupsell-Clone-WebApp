using CupsellCloneAPI.Authentication.Models;

namespace CupsellCloneAPI.Authentication.Services
{
    public interface IAccountService
    {
        Task RegisterUser(RegisterUserDto dto);
        Task<string> GenerateJwt(LoginUserDto dto);
        string GenerateRefreshTokenAndSave(Guid userId);
    }
}