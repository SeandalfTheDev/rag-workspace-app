using CleverDocs.Shared.Authentication.Models;
using FluentValidation;

namespace CleverDocs.Core.Validation.Authentication;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
        .NotEmpty()
        .EmailAddress()
        .WithMessage("Email is required and must be a valid email address");

        RuleFor(x => x.Password)
        .NotEmpty()
        .MinimumLength(8)
        .WithMessage("Password is required and must be at least 8 characters long");

        RuleFor(x => x.FirstName)
        .NotEmpty()
        .WithMessage("First name is required");

        RuleFor(x => x.LastName)
        .NotEmpty()
        .WithMessage("Last name is required");
    }
}