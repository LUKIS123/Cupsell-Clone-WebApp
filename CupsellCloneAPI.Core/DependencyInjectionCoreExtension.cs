using CupsellCloneAPI.Core.Services;
using CupsellCloneAPI.Core.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CupsellCloneAPI.Core;

public static class DependencyInjectionCoreExtension
{
    public static IServiceCollection AddCoreServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IHttpContextService, HttpContextService>();
        serviceCollection.AddScoped<IImageService, ImageService>();
        serviceCollection.AddScoped<IOfferService, OfferService>();

        return serviceCollection;
    }

    public static IServiceCollection AddAutoMapper(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(Assembly.GetExecutingAssembly());
        return serviceCollection;
    }
}