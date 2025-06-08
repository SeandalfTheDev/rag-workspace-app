using CleverDocs.Core.Entities;
using CleverDocs.Shared.Abstractions;
using CleverDocs.Shared.Authentication.Models;
using Microsoft.AspNetCore.Authentication;

namespace CleverDocs.Core.Abstractions.Authentication;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
    Task<User?> GetUserByEmailAsync(string email);
}