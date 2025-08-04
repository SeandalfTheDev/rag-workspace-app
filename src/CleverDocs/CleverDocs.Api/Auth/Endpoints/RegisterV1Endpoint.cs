using CleverDocs.Application.Auth.DTOs;
using Microsoft.AspNetCore.Http;
using CleverDocs.Application.Auth.Interfaces;
using CleverDocs.Domain.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CleverDocs.Api.Auth.Endpoints;

public static class RegisterV1Endpoint
{
    public static void MapRegistrationV1Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("register", Handler)
            .WithName("RegisterUser")
            .WithSummary("Registers a new user.")
            .WithDescription("Creates a new user account and returns JWT access and refresh tokens.")
            .Produces<AccessTokensDto>()
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .WithOpenApi();
    }

    private static async Task<IResult> Handler(
        HttpContext context,
        RegisterUserDto registerUserDto,
        IValidator<RegisterUserDto> validator,
        IAuthService authService)
    {
        var validationResult = await validator.ValidateAsync(registerUserDto);
        if (!validationResult.IsValid)
        {   
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }
        
        var emailExists = await authService.EmailExistsAsync(registerUserDto.Email);
        if (emailExists)
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Registration failed.",
                detail: "A user with this email address already exists."
            );
        }
        
        var authResult = await authService.RegisterUserAsync(registerUserDto);
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