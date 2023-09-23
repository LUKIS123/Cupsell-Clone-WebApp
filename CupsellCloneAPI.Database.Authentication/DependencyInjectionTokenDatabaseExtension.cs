using CupsellCloneAPI.Database.Authentication.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CupsellCloneAPI.Database.Authentication
{
    public static class DependencyInjectionTokenDatabaseExtension
    {
        public static IServiceCollection AddTokenDatabaseRepository(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            return serviceCollection;
        }
    }
}