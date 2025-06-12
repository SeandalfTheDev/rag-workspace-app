
using CleverDocs.Core.Abstractions.Authentication;
using CleverDocs.Shared.Authentication.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CleverDocs.WebApi.Modules.Authentication.Endpoints;

public static class RegisterEndpoint
{
    public static void MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register", RegisterHandler)
            .AllowAnonymous()
            .WithTags("Authentication")
            .WithSummary("Register a new user")
            .WithDescription("Register a new user")
            .WithOpenApi()
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> RegisterHandler(
        HttpContext context,
        RegisterRequest request,
        IAuthService authService,
        IValidator<RegisterRequest> validator,
        Serilog.ILogger logger
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var result = await authService.RegisterAsync(request);
        if (!result.IsSuccess)
        {
            return Results.BadRequest(result.Error);
        }

        return Results.Ok(result.Data);
    }
}