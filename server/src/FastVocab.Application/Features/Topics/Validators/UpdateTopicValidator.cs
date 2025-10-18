using FastVocab.Application.Features.Topics.Commands.UpdateTopic;
using FluentValidation;

namespace FastVocab.Application.Features.Topics.Validators;

/// <summary>
/// Validator for UpdateTopicCommand
/// </summary>
public class UpdateTopicValidator : AbstractValidator<UpdateTopicCommand>
{
    public UpdateTopicValidator()
    {
        RuleFor(x => x.Request.Id)
            .GreaterThan(0).WithMessage("Topic ID must be greater than 0.");

        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage("Topic name (English) is required.")
            .MinimumLength(2).WithMessage("Topic name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Topic name must not exceed 100 characters.");

        RuleFor(x => x.Request.VnText)
            .NotEmpty().WithMessage("Topic name (Vietnamese) is required.")
            .MinimumLength(2).WithMessage("Vietnamese name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Vietnamese name must not exceed 100 characters.");

        RuleFor(x => x.Request.ImageUrl)
            .MaximumLength(500).WithMessage("Image URL must not exceed 500 characters.")
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.Request.ImageUrl))
            .WithMessage("Image URL must be a valid URL.");
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}

