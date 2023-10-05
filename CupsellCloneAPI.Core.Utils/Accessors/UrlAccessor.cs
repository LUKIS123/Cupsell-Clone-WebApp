using Microsoft.AspNetCore.Http;

namespace CupsellCloneAPI.Core.Utils.Accessors
{
    public class UrlAccessor : IUrlAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UrlAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpRequest Request => _httpContextAccessor.HttpContext.Request;

        public string BaseUrl => Request.Scheme + "://" + Request.Host;

        public string GetControllerPath(string controllerName)
        {
            var path = Request.Path.Value;
            return path[..(path.IndexOf(controllerName, StringComparison.Ordinal) + controllerName.Length)] + "/";
        }
    }
}