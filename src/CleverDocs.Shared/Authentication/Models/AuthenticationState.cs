using CleverDocs.Shared.Authentication.Dtos;

namespace CleverDocs.Shared.Authentication.Models;

public record AuthenticationState
{
    public bool IsAuthenticated { get; init; }
    public UserDto? User { get; init; }
    public string? Token { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? TokenExpiresAt { get; init; }

    public static AuthenticationState Unauthenticated => new() { IsAuthenticated = false };
    
    public static AuthenticationState Authenticated(UserDto user, string token, string refreshToken, DateTime tokenExpiresAt) =>
        new()
        {
            IsAuthenticated = true,
            User = user,
            Token = token,
            RefreshToken = refreshToken,
            TokenExpiresAt = tokenExpiresAt
        };
} 