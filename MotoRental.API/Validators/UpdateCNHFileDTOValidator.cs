using FluentValidation;
using MotoRental.API.ConfigController;
using MotoRental.Core.DTO.DeliveryPersonService;

namespace MotoRental.API.Validators
{
    public class UpdateCNHFileDTOValidator : AbstractValidatorCustom<UpdateCNHFileDTO>
    {
        public UpdateCNHFileDTOValidator()
        {
            When(w => w is not null, () =>
            {
                RuleFor(x => x.File)
                .NotNull().WithMessage("File is required.")
                .Must(file => file.ContentType == "image/png" || file.ContentType == "image/bmp")
                .WithMessage("Only PNG or JPG files are allowed.")
                .Must(file => file.Length <= 1024 * 1024 * 5)
                .WithMessage("File size must be under 5MB.");
            });
        }
    }
}
