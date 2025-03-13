using System.Text.RegularExpressions;
using FluentValidation;

namespace AdvertisingPlatforms.Presentation.Validators;

public partial class LocationRequestValidator : AbstractValidator<string>
{
    public LocationRequestValidator()
    {
        RuleFor(x => x)
            .NotNull().WithMessage("Location can't be null")
            .NotEmpty().WithMessage("Location can't be empty")
            .Must(x => 
                LocationRequestRegex().Match(x).Success).WithMessage("The location has an incorrect format");
    }

    [GeneratedRegex(@"^(/[\w_-]+)+$")]
    private static partial Regex LocationRequestRegex();
}