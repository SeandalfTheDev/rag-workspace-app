using CleverDocs.Api.Auth.Endpoints;

namespace CleverDocs.Api.Auth;

public static class AuthModule
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .AllowAnonymous()
            .WithTags("Authentication");
        
        group.MapRegistrationV1Endpoint();
        group.MapLoginV1Endpoint();

        return app;
    }
}