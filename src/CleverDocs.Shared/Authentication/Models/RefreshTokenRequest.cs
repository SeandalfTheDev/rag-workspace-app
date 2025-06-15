namespace CleverDocs.Shared.Authentication.Models;

public record RefreshTokenRequest
{
    public required string RefreshToken { get; init; }
} 