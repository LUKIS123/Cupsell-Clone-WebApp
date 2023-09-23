using CupsellCloneAPI.Authentication.Exceptions;
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
            catch (ForbidException forbidException)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync(forbidException.Message);
            }
            catch (InvalidLoginParamsException invalidParamsException)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(invalidParamsException.Message);
            }
            catch (BlobFileAlreadyExistsException blobFileAlreadyExists)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(blobFileAlreadyExists.Message);
            }
            catch (BlobFileNotFoundException blobFileNotFound)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(blobFileNotFound.Message);
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