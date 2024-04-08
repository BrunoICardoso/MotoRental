using FluentValidation;
using MotoRental.API.ConfigController;
using MotoRental.Core.DTO.RentalService;

namespace MotoRental.API.Validators
{
    public class RentalReturnDTOValidator : AbstractValidatorCustom<RentalReturnDTO>
    {
        public RentalReturnDTOValidator()
        {
            When(w => w is not null, () =>
            {
                RuleFor(r => r.EndDate)
                 .GreaterThanOrEqualTo(DateTime.Now).WithMessage("End Date cannot be in the past");
            });
        }
    }
}
