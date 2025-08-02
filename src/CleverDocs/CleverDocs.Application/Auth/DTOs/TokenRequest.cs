namespace CleverDocs.Application.Auth.DTOs;

public record TokenRequest(string UserId, string Email, IEnumerable<string> Roles);