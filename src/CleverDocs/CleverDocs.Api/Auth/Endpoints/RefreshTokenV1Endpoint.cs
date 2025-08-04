using CleverDocs.Application.Auth.DTOs;
using CleverDocs.Application.Auth.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CleverDocs.Api.Auth.Endpoints;

public static class RefreshTokenV1Endpoint
{
    public static void MapRefreshTokenV1Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("refresh", Handler)
            .WithName("Refresh Access Token")
            .WithSummary("Refreshes the access token using a refresh token.")
            .WithDescription("Refreshes the access token using a refresh token and returns new access tokens.")
            .Produces<AccessTokensDto>()
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .WithOpenApi();
    }

    private static async Task<IResult> Handler(
        HttpContext context,
        RefreshTokenDto refreshTokenDto,
        IValidator<RefreshTokenDto> validator,
        IAuthService authService)
    {
        var validationResult = await validator.ValidateAsync(refreshTokenDto);
        if (!validationResult.IsValid)
        {   
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }
        
        var authResult = await authService.RefreshTokenAsync(refreshTokenDto);
        if (authResult.IsFailure)
        {
            return TypedResults.Unauthorized();
        }
        
        return TypedResults.Ok(authResult.Value);
    }
}