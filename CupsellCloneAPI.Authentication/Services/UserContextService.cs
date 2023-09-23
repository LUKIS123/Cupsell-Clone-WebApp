using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace CupsellCloneAPI.Authentication.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContext;

    public UserContextService(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
    }

    public ClaimsPrincipal? User => _httpContext.HttpContext?.User;

    public Guid? GetUserId
    {
        get
        {
            var value = User?.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (value != null)
            {
                return (Guid?)Guid.Parse(value);
            }

            return null;
        }
    }
}