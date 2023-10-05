using CupsellCloneAPI.Authentication.Exceptions;
using CupsellCloneAPI.Core.Exceptions;
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
                await context.Response.WriteAsJsonAsync(forbidException.Message);
            }
            catch (AuthenticationErrorException authenticationErrorException)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(authenticationErrorException.Message);
            }
            catch (InvalidAuthenticationParamsException invalidParamsException)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(invalidParamsException.Message);
            }
            catch (BlobFileAlreadyExistsException blobFileAlreadyExists)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(blobFileAlreadyExists.Message);
            }
            catch (BlobFileNotFoundException blobFileNotFound)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsJsonAsync(blobFileNotFound.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync("Something went wrong...");
            }
        }
    }
}