using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CupsellCloneAPI.Core.Utils.Accessors
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal? UserClaimsPrincipal => _httpContextAccessor.HttpContext?.User;

        public Guid? UserId
        {
            get
            {
                var value = UserClaimsPrincipal?.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (value == null) return null;
                if (!Guid.TryParse(value, out var userId))
                {
                    return null;
                }

                return userId;
            }
        }

        public string? Email
        {
            get
            {
                var value = UserClaimsPrincipal?.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
                return value;
            }
        }
    }
}