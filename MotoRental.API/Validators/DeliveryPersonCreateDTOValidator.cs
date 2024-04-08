using FluentValidation;
using MotoRental.API.ConfigController;
using MotoRental.Core.DTO.DeliveryPersonService;
using MotoRental.Infrastructure.Help;

namespace MotoRental.API.Validators
{
    public class DeliveryPersonCreateDTOValidator : AbstractValidatorCustom<DeliveryPersonCreateDTO>
    {
        public DeliveryPersonCreateDTOValidator()
        {
            When(w => w is not null, () =>
            {

                RuleFor(r => r.CNPJ)
               .NotEmpty().WithMessage("The CNPJ cannot be empty.")
               .Must(x => x.IsCNPJ()).WithMessage("The CNPJ is not valid.");

                RuleFor(r => r.CNHNumber)
                    .NotEmpty().WithMessage("The CNH Number cannot be empty.")
                    .Must(x => x.IsCNH()).WithMessage("The CNH Number is not valid.");

                RuleFor(r => r.CNHType)
                    .NotEmpty().WithMessage("The CNH Type cannot be empty.")
                    .IsInEnum().WithMessage("The CNH Type is not a valid value.");

                RuleFor(r => r.DateOfBirth)
                    .NotEmpty().WithMessage("The Date of Birth cannot be empty.")
                    .Must(BeAValidAge).WithMessage("The person must be at least 18 years old.");

                RuleFor(r => r.Name)
                    .NotEmpty().WithMessage("The Name cannot be empty.");

            });
        }

        private bool BeAValidAge(DateTime date)
        {
            int minimumAge = 18;
            return date <= DateTime.Today.AddYears(-minimumAge);
        }
    }
}
