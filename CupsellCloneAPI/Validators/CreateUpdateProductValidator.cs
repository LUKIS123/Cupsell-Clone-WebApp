using CupsellCloneAPI.Core.Models.Dtos.Product;
using CupsellCloneAPI.Database.Repositories.Interfaces;
using FluentValidation;

namespace CupsellCloneAPI.Validators
{
    public class CreateUpdateProductValidator : AbstractValidator<CreateProductDto>
    {
        public CreateUpdateProductValidator(IProductRepository productRepository)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(512);

            RuleFor(x => x.ProductTypeName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.ProductTypeName)
                .CustomAsync(async (value, context, _) =>
                {
                    var productTypes = await productRepository.GetProductTypes();
                    if (productTypes.All(x => !string.Equals(x.Name, value, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        context.AddFailure("ProductTypeName", $"Product type:{value} does not exist");
                    }
                });
        }
    }
}