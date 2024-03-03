namespace CupsellCloneAPI.Middleware
{
    public class ApiEndpointMiddleware : IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = 404;
                return Task.CompletedTask;
            }

            return next.Invoke(context);
        }
    }
}