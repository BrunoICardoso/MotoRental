using FluentValidation;
using MotoRental.API.ConfigController;
using MotoRental.Core.DTO.AuthService;
using MotoRental.Core.DTO.OrderService;

namespace MotoRental.API.Validators
{
    public class OrderCreateDTOValidator : AbstractValidatorCustom<OrderCreateDTO>
    {
        public OrderCreateDTOValidator()
        {
            When(w => w is not null, () =>
            {
                RuleFor(x => x.RaceValue)
                  .NotNull().WithMessage("Race value is required.")
                  .GreaterThan(0).WithMessage("Race value must be greater than 0.");
            });
        }
    }
}
