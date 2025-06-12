using CleverDocs.WebApi.Modules.Authentication.Endpoints;

namespace CleverDocs.WebApi.Modules.Authentication;

public static class AuthenticationModules
{
  public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
  {
    app.MapRegisterEndpoint();
    app.MapLoginEndpoint();
  }
}