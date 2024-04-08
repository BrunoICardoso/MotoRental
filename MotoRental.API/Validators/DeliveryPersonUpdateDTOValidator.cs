using FluentValidation;
using MotoRental.API.ConfigController;
using MotoRental.Core.DTO.DeliveryPersonService;
using MotoRental.Infrastructure.Help;

namespace MotoRental.API.Validators
{
    public class DeliveryPersonUpdateDTOValidator : AbstractValidatorCustom<DeliveryPersonUpdateDTO>
    {
        public DeliveryPersonUpdateDTOValidator()
        {
            When(w => w is not null, () =>
            {

                RuleFor(r => r.CNHNumber)
                .Must(cnh => string.IsNullOrEmpty(cnh) || cnh.IsCNH()).WithMessage("Invalid CNH Number format.")
                .When(r => !string.IsNullOrEmpty(r.CNHNumber));

                RuleFor(r => r.CNHType)
                    .IsInEnum().WithMessage("Invalid CNH Type.")
                    .When(r => r.CNHType.HasValue);

                RuleFor(r => r.DateOfBirth)
                    .Must(date => !date.HasValue || date.Value < DateTime.Today).WithMessage("Date of Birth must be in the past.")
                    .Must(date => !date.HasValue || (date.Value.Year > 1900 && date.Value <= DateTime.Today)).WithMessage("Date of Birth is not reasonable.")
                    .When(r => r.DateOfBirth.HasValue);
            });
        }

    }
}
