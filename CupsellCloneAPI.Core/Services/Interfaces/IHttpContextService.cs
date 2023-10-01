using System.Security.Claims;

namespace CupsellCloneAPI.Core.Services.Interfaces
{
    public interface IHttpContextService
    {
        ClaimsPrincipal? User { get; }
        Guid? GetUserId { get; }
    }
}