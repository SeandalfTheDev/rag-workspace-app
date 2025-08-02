using CleverDocs.Application.Auth.DTOs;
using FluentValidation;

namespace CleverDocs.Application.Auth.Validation;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);       

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(100);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match");
    }
}