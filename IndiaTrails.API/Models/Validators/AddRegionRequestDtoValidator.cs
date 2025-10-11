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
}
