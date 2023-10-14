using CupsellCloneAPI.Authentication.Models;
using CupsellCloneAPI.Database.Repositories.Interfaces;
using FluentValidation;

namespace CupsellCloneAPI.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator(IUserRepository userRepository)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .MinimumLength(8);

            RuleFor(x => x.ConfirmPassword)
                .Equal(e => e.Password);

            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    var user = userRepository.GetByEmail(value);
                    if (user.Result is not null)
                    {
                        context.AddFailure("Email", "Email already taken");
                    }
                });

            RuleFor(x => x.RoleId)
                .InclusiveBetween(1, 2);
        }
    }
}