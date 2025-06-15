using CleverDocs.Client.Models;

namespace CleverDocs.Client.Services;

public interface IAuthenticationService
{
    /// <summary>
    /// Gets the current authentication state
    /// </summary>
    AuthenticationState CurrentState { get; }
    
    /// <summary>
    /// Event that fires when authentication state changes
    /// </summary>
    event Action<AuthenticationState>? AuthenticationStateChanged;
    
    /// <summary>
    /// Attempts to log in with email and password
    /// </summary>
    Task<(bool Success, string? ErrorMessage)> LoginAsync(string email, string password, bool rememberMe = false);
    
    /// <summary>
    /// Attempts to register a new user
    /// </summary>
    Task<(bool Success, string? ErrorMessage)> RegisterAsync(string firstName, string lastName, string email, string password);
    
    /// <summary>
    /// Logs out the current user
    /// </summary>
    Task LogoutAsync();
    
    /// <summary>
    /// Refreshes the current authentication token
    /// </summary>
    Task<bool> RefreshTokenAsync();
    
    /// <summary>
    /// Initializes the authentication service and restores state from storage
    /// </summary>
    Task InitializeAsync();
    
    /// <summary>
    /// Checks if the current token is valid and not expired
    /// </summary>
    bool IsTokenValid();
} 