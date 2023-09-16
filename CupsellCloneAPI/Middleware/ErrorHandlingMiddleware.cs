using CupsellCloneAPI.Database.BlobContainer.Exceptions;

namespace CupsellCloneAPI.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (BlobFileAlreadyExistsException blobFileAlreadyExists)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(blobFileAlreadyExists.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Something went wrong...");
            }
        }
    }
}