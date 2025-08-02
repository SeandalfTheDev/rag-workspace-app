using CleverDocs.Application.Auth.DTOs;

namespace CleverDocs.Application.Auth.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Registers a new user with the provided registration details
    /// </summary>
    /// <param name="registerUserDto">The registration details</param>
    /// <returns>Access tokens for the newly registered user</returns>
    Task<AuthResult<AccessTokensDto>> RegisterUserAsync(RegisterUserDto registerUserDto);
    
    /// <summary>
    /// Logs in a user with the provided login details
    /// </summary>
    /// <param name="loginUserDto">The login details</param>
    /// <returns>Access tokens for the logged-in user</returns>
    Task<AuthResult<AccessTokensDto>> LoginUserAsync(LoginUserDto loginUserDto);

    /// <summary>
    /// Checks if an email address is already registered in the system
    /// </summary>
    /// <param name="email">The email address to check</param>
    /// <returns>True if the email exists, otherwise false</returns>
    Task<bool> EmailExistsAsync(string email);
}