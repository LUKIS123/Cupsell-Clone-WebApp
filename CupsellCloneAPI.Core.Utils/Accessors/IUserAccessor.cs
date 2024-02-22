using System.Security.Claims;

namespace CupsellCloneAPI.Core.Utils.Accessors
{
    public interface IUserAccessor
    {
        Guid? UserId { get; }
        string? Email { get; }
        ClaimsPrincipal? UserClaimsPrincipal { get; }
    }
}