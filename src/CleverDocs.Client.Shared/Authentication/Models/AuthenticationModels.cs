using CleverDocs.Client.Shared.Authentication.Dtos;

namespace CleverDocs.Client.Shared.Authentication.Models;

public record LoginRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public record RegisterRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public record AuthResponse
{
    public required string Token { get; init; }
    public required string RefreshToken { get; init; }
    public required UserDto User { get; init; }
}

public record RefreshTokenRequest
{
    public required string RefreshToken { get; init; }
}

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