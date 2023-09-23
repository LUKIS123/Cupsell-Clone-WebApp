using System.Security.Claims;

namespace CupsellCloneAPI.Authentication.Services
{
    public interface IUserContextService
    {
        ClaimsPrincipal? User { get; }
        Guid? GetUserId { get; }
    }
}