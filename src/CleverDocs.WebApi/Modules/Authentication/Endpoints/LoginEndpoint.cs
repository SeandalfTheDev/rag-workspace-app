using CleverDocs.Core.Abstractions.Authentication;
using CleverDocs.Shared.Authentication.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CleverDocs.WebApi.Modules.Authentication.Endpoints;

public static class LoginEndpoint
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", LoginHandler)
            .WithTags("Authentication")
            .WithSummary("Login a user")
            .WithDescription("Login a user")
            .WithOpenApi()
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    private static async Task LoginHandler(
        HttpContext context,
        LoginRequest request,
        IAuthService authService,
        IValidator<LoginRequest> validator,
        Serilog.ILogger logger
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            Results.BadRequest(validationResult.Errors);
        }

        var result = await authService.LoginAsync(request);
        if (!result.IsSuccess)
        {
            Results.BadRequest(result.Error);
        }

        Results.Ok(result.Data);
    }
}