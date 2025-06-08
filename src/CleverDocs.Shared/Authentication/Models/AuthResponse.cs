using CleverDocs.Shared.Authentication.Dtos;

namespace CleverDocs.Shared.Authentication.Models;

public record AuthResponse
{
    public required string Token { get; init; }
    public required string RefreshToken { get; init; }
    public required UserDto User { get; init; }
}