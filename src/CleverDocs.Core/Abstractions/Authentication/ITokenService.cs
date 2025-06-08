using System.Security.Claims;
using CleverDocs.Core.Entities;

namespace CleverDocs.Core.Abstractions.Authentication;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
    Task<bool> IsTokenValidAsync(string token);
}