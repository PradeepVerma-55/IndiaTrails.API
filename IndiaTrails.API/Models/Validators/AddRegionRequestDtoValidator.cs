using FluentValidation;
using IndiaTrails.API.Models.DTOs.Request;

namespace IndiaTrails.API.Models.Validators
{
    public class AddRegionRequestDtoValidator : AbstractValidator<AddRegionRequestDto>
    {
        public AddRegionRequestDtoValidator()
        {
           RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Region name is required.")
                .MaximumLength(100);

           RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Region code is required.")
                .MaximumLength(10);

           RuleFor(x => x.RegionImageUrl)
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrWhiteSpace(x.RegionImageUrl))
                .WithMessage("RegionImageUrl must be a valid URL.");

        }
    }

    public class AddWalkRequestDtoValidator : AbstractValidator<AddWalkRequestDto>
    {
        public AddWalkRequestDtoValidator()
        {
            RuleFor(x => x.Name)
                 .NotEmpty().WithMessage("Name is required.")
                 .MaximumLength(100);

            RuleFor(x => x.Description)
                 .NotEmpty().WithMessage("Description is required.")
                 .MaximumLength(1000);

            RuleFor(x => x.LengthInKm)
                 .GreaterThan(0).WithMessage("LengthInKm must be greater than zero.");
        }
    }
}
