using CleverDocs.Application.Auth.DTOs;
using CleverDocs.Application.Auth.Interfaces;
using CleverDocs.Domain.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CleverDocs.Api.Auth.Endpoints;

public static class LoginV1Endpoint
{
    public static void MapLoginV1Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("login", Handler)
            .WithName("Login User")
            .WithSummary("Login an existing user.")
            .WithDescription("Logs a User in and returns JWT access and refresh tokens.")
            .Produces<AccessTokensDto>()
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .WithOpenApi();
    }

    private static async Task<IResult> Handler(
        HttpContext context,
        LoginUserDto loginUserDto,
        IValidator<LoginUserDto> validator,
        IAuthService authService)
    {
        var validationResult = await validator.ValidateAsync(loginUserDto);
        if (!validationResult.IsValid)
        {   
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }
        
        var emailExists = await authService.EmailExistsAsync(loginUserDto.Email);
        if (!emailExists)
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Login failed.",
                detail: "A user with this email address does not exist."
            );
        }

        var authResult = await authService.LoginUserAsync(loginUserDto);
        if (authResult.IsFailure)
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Registration failed.",
                detail: authResult.Error.Description);
        }
        
        return TypedResults.Ok(authResult.Value);
    }
}