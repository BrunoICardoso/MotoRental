using FluentValidation;
using MotoRental.API.ConfigController;
using MotoRental.Core.DTO.AuthService;
using MotoRental.Core.DTO.DeliveryPersonService;

namespace MotoRental.API.Validators
{
    public class LoginDTOValidator : AbstractValidatorCustom<LoginDTO>
    {
        public LoginDTOValidator()
        {
            When(w => w is not null, () =>
            {
                RuleFor(r => r.Username)
                .NotEmpty().WithMessage("Username is required");

                RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Password is required");

            });
        }
    }
}
