using FluentValidation;
using MotoRental.API.ConfigController;
using MotoRental.Core.DTO.DeliveryPersonService;
using MotoRental.Core.DTO.MotoService;

namespace MotoRental.API.Validators
{
    public class MotoCreateDTOValidator : AbstractValidatorCustom<MotoCreateDTO>
    {
        public MotoCreateDTOValidator()
        {
            When(w => w is not null, () =>
            {
                RuleFor(r => r.Model)
                .NotEmpty().WithMessage("Model is required");

                RuleFor(r => r.Year)
                .NotEmpty()
                .InclusiveBetween(1900, DateTime.Now.Year).WithMessage("Year must be between 1900 and current year");

                RuleFor(r => r.LicensePlate)
                    .NotEmpty().WithMessage("License Plate is required")
                    .Matches("^[A-Za-z0-9]{7}$").WithMessage("License Plate must be 7 alphanumeric characters");
            });
        }
    }
}
