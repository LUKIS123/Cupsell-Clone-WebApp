using CupsellCloneAPI.Database.Authentication.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CupsellCloneAPI.Database.Authentication
{
    public static class DependencyInjectionTokenDatabaseExtension
    {
        public static IServiceCollection AddTokenDatabaseRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            serviceCollection.AddScoped<IVerificationTokenRepository, VerificationTokenRepository>();
            return serviceCollection;
        }
    }
}