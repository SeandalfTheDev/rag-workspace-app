using CleverDocs.Application.Auth.DTOs;
using FluentValidation;

namespace CleverDocs.Application.Auth.Validation;

public class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
{
    public RefreshTokenDtoValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
