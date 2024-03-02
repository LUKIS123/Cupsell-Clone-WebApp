using CupsellCloneAPI.Database.Authentication.Models;

namespace CupsellCloneAPI.Database.Authentication.Repositories
{
    public interface IVerificationTokenRepository
    {
        Task<VerificationTokenDto?> GetByToken(string verificationToken);
        Task<VerificationTokenDto?> GetByUserId(Guid userId);
        Task Create(Guid userId, string verificationToken);
        Task DeleteByUserId(Guid userId);
    }
}