using CupsellCloneAPI.Database.Factory;
using CupsellCloneAPI.Database.Repositories;
using CupsellCloneAPI.Database.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CupsellCloneAPI.Database;

public static class DependencyInjectionDatabaseExtension
{
    public static IServiceCollection AddSqlConnectionFactory(
        this IServiceCollection serviceCollection,
        string connectionString
    )
    {
        serviceCollection.AddScoped<IDbConnectionFactory>(provider =>
            new DbConnectionFactory(connectionString)
        );
        return serviceCollection;
    }

    public static IServiceCollection AddModelRepositories(
        this IServiceCollection serviceCollection
    )
    {
        serviceCollection.AddScoped<IUserRepository, UserRepository>();
        serviceCollection.AddScoped<IOfferRepository, OfferRepository>();
        serviceCollection.AddScoped<IProductRepository, ProductRepository>();
        serviceCollection.AddScoped<IGraphicRepository, GraphicRepository>();
        serviceCollection.AddScoped<IAvailableItemsRepository, AvailableItemsRepository>();

        return serviceCollection;
    }
}