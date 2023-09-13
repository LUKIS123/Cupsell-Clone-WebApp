using Microsoft.Extensions.DependencyInjection;

namespace CupsellCloneAPI.Core;

public static class DependencyInjectionCoreExtension
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection;
    }
}