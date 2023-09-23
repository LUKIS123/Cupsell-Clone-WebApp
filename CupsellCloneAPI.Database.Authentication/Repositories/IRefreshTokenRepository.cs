using CupsellCloneAPI.Database.Authentication.Models;

namespace CupsellCloneAPI.Database.Authentication.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshTokenDto> GetByToken(string token);
        Task<Guid> Create(Guid userId, string refreshToken);
        Task Delete(Guid id);
        Task DeleteByUserId(Guid userId);
    }
}