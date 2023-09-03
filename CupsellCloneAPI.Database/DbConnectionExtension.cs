using Microsoft.Extensions.DependencyInjection;

namespace CupsellCloneAPI.Database
{
    public static class DbConnectionExtension
    {
        public static IServiceCollection AddSqlConnectionFactory(
            this IServiceCollection serviceCollection,
            string connectionString
        )
        {
            serviceCollection.AddScoped<IDbConnectionFactory>(provider =>
                new SqlDbConnectionFactory(connectionString)
            );
            return serviceCollection;
        }
    }
}