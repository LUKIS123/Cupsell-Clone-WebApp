using System.Security.Claims;
using CupsellCloneAPI.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CupsellCloneAPI.Core.Services;

public class HttpContextService : IHttpContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? GetUserId
    {
        get
        {
            var value = User?.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (value == null) return null;
            if (!Guid.TryParse(value, out var userId))
            {
                return null;
            }

            return userId;
        }
    }

    public string GetBaseUrl
    {
        get
        {
            var request = _httpContextAccessor.HttpContext.Request;
            // var url = _httpContextAccessor.HttpContext.
            return request.Scheme + "://" + request.Host;
        }
    }
}