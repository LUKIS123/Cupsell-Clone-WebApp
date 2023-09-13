using CupsellCloneAPI.Database.BlobContainer.Factory;
using CupsellCloneAPI.Database.BlobContainer.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CupsellCloneAPI.Database.BlobContainer;

public static class DependencyInjectionBlobContainerExtension
{
    public static IServiceCollection AddBlobStorage(
        this IServiceCollection serviceCollection,
        string connectionString, string container
    )
    {
        serviceCollection.AddSingleton<IBlobServiceClientFactory>(provider =>
            new BlobServiceClientFactory(connectionString, container));

        serviceCollection.AddScoped<IBlobRepository, BlobRepository>();

        return serviceCollection;
    }
}