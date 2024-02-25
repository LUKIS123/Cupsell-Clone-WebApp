using CupsellCloneAPI.Authentication.Models;
using CupsellCloneAPI.Core.Models.Dtos.Product;
using FluentValidation;

namespace CupsellCloneAPI.Validators
{
    public static class DependencyInjectionValidatorsExtension
    {
        public static void AddValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
            services.AddScoped<IValidator<CreateProductDto>, CreateUpdateProductValidator>();
        }
    }
}