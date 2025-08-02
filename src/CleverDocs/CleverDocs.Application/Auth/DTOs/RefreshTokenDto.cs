namespace CleverDocs.Application.Auth.DTOs;

public record RefreshTokenDto
{
    public required string RefreshToken { get; init; }
}