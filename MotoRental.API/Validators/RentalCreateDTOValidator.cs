using FluentValidation;
using MotoRental.API.ConfigController;
using MotoRental.Core.DTO.MotoService;
using MotoRental.Core.DTO.RentalService;

namespace MotoRental.API.Validators
{
    public class RentalCreateDTOValidator : AbstractValidatorCustom<RentalCreateDTO>
    {
        public RentalCreateDTOValidator()
        {
            When(w => w is not null, () =>
            {
                RuleFor(r => r.DeliveryPersonId)
                .NotNull()
                .NotEmpty()
                .GreaterThan(0).WithMessage("Invalid Delivery Person ID");

                RuleFor(r => r.MotoId)
                .NotNull()
                .NotEmpty()
                    .GreaterThan(0).WithMessage("Invalid Moto ID");

                RuleFor(r => r.StartDate)
                .NotNull()
                .NotEmpty()
                .GreaterThanOrEqualTo(DateTime.Now).WithMessage("Start Date must be today or later");

                RuleFor(r => r.Plan)
                      .NotNull()
                      .NotEmpty()
                      .IsInEnum();

            });
        }
    }
}
