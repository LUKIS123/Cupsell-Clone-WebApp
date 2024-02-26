using CupsellCloneAPI.Core.Services;
using CupsellCloneAPI.Core.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CupsellCloneAPI.Core;

public static class DependencyInjectionCoreExtension
{
    public static IServiceCollection AddCoreServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAssetsService, AssetsService>();
        serviceCollection.AddScoped<IOfferService, OfferService>();
        serviceCollection.AddScoped<IProductService, ProductService>();
        serviceCollection.AddScoped<IGraphicService, GraphicService>();

        return serviceCollection;
    }

    public static IServiceCollection AddAutoMapper(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(Assembly.GetExecutingAssembly());
        return serviceCollection;
    }
}