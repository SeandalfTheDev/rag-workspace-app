using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.Json;
using CleverDocs.Client.Models;

namespace CleverDocs.Client.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly IWebAssemblyHostEnvironment _environment;
    
    private AuthenticationState _currentState = AuthenticationState.Unauthenticated;
    private readonly string _tokenStorageKey = "cleverdocs_auth_token";
    private readonly string _refreshTokenStorageKey = "cleverdocs_refresh_token";
    private readonly string _userStorageKey = "cleverdocs_user";

    public AuthenticationState CurrentState => _currentState;
    public event Action<AuthenticationState>? AuthenticationStateChanged;

    public AuthenticationService(
        HttpClient httpClient, 
        IJSRuntime jsRuntime,
        IWebAssemblyHostEnvironment environment)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _environment = environment;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var token = await GetFromStorageAsync(_tokenStorageKey);
            var refreshToken = await GetFromStorageAsync(_refreshTokenStorageKey);
            var userJson = await GetFromStorageAsync(_userStorageKey);

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userJson))
            {
                var user = JsonSerializer.Deserialize<UserDto>(userJson);

                if (user != null)
                {
                    // For simplicity, assume token is valid for 24 hours from now
                    var expiresAt = DateTime.UtcNow.AddHours(24);
                    _currentState = AuthenticationState.Authenticated(user, token, refreshToken ?? "", expiresAt);
                    
                    // Set the authorization header
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    
                    NotifyAuthenticationStateChanged();
                }
            }
        }
        catch (Exception ex)
        {
            // Log error in development
            if (_environment.IsDevelopment())
            {
                Console.WriteLine($"Error initializing authentication: {ex.Message}");
            }
            
            // Clear any corrupted storage data
            await LogoutAsync();
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> LoginAsync(string email, string password, bool rememberMe = false)
    {
        try
        {
            var loginRequest = new LoginRequest { Email = email, Password = password };
            
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginRequest);
            
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                
                if (authResponse != null)
                {
                    await SetAuthenticationStateAsync(authResponse, rememberMe);
                    return (true, null);
                }
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return (false, GetErrorMessage(errorContent) ?? "Login failed. Please check your credentials.");
        }
        catch (HttpRequestException)
        {
            return (false, "Unable to connect to the server. Please check your internet connection.");
        }
        catch (Exception ex)
        {
            if (_environment.IsDevelopment())
            {
                Console.WriteLine($"Login error: {ex.Message}");
            }
            return (false, "An unexpected error occurred. Please try again.");
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(string firstName, string lastName, string email, string password)
    {
        try
        {
            var registerRequest = new RegisterRequest 
            { 
                FirstName = firstName, 
                LastName = lastName, 
                Email = email, 
                Password = password 
            };
            
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", registerRequest);
            
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                
                if (authResponse != null)
                {
                    await SetAuthenticationStateAsync(authResponse, false);
                    return (true, null);
                }
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return (false, GetErrorMessage(errorContent) ?? "Registration failed. Please try again.");
        }
        catch (HttpRequestException)
        {
            return (false, "Unable to connect to the server. Please check your internet connection.");
        }
        catch (Exception ex)
        {
            if (_environment.IsDevelopment())
            {
                Console.WriteLine($"Registration error: {ex.Message}");
            }
            return (false, "An unexpected error occurred. Please try again.");
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            // Call logout endpoint if authenticated
            if (_currentState.IsAuthenticated)
            {
                await _httpClient.PostAsync("/api/auth/logout", null);
            }
        }
        catch
        {
            // Ignore logout API errors - we'll clear local state anyway
        }
        finally
        {
            // Clear local storage
            await RemoveFromStorageAsync(_tokenStorageKey);
            await RemoveFromStorageAsync(_refreshTokenStorageKey);
            await RemoveFromStorageAsync(_userStorageKey);
            
            // Clear authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;
            
            // Update state
            _currentState = AuthenticationState.Unauthenticated;
            NotifyAuthenticationStateChanged();
        }
    }

    public async Task<bool> RefreshTokenAsync()
    {
        if (string.IsNullOrEmpty(_currentState.RefreshToken))
            return false;

        try
        {
            var refreshRequest = new RefreshTokenRequest { RefreshToken = _currentState.RefreshToken };
            var response = await _httpClient.PostAsJsonAsync("/api/auth/refresh", refreshRequest);
            
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                
                if (authResponse != null)
                {
                    await SetAuthenticationStateAsync(authResponse, true);
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            if (_environment.IsDevelopment())
            {
                Console.WriteLine($"Token refresh error: {ex.Message}");
            }
        }

        // Refresh failed, logout user
        await LogoutAsync();
        return false;
    }

    public bool IsTokenValid()
    {
        if (!_currentState.IsAuthenticated || string.IsNullOrEmpty(_currentState.Token))
            return false;

        // Check if token has expired based on stored expiration time
        return _currentState.TokenExpiresAt.HasValue && 
               _currentState.TokenExpiresAt.Value > DateTime.UtcNow.AddMinutes(5); // 5 minute buffer
    }

    private async Task SetAuthenticationStateAsync(AuthResponse authResponse, bool rememberMe)
    {
        // For simplicity, assume token is valid for 1 hour
        var tokenExpiresAt = DateTime.UtcNow.AddHours(1);
        
        _currentState = AuthenticationState.Authenticated(
            authResponse.User, 
            authResponse.Token, 
            authResponse.RefreshToken, 
            tokenExpiresAt);

        // Set authorization header
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse.Token);

        // Store in local storage if remember me is checked
        if (rememberMe)
        {
            await SetStorageAsync(_tokenStorageKey, authResponse.Token);
            await SetStorageAsync(_refreshTokenStorageKey, authResponse.RefreshToken);
            await SetStorageAsync(_userStorageKey, JsonSerializer.Serialize(authResponse.User));
        }

        NotifyAuthenticationStateChanged();
    }

    private string? GetErrorMessage(string errorContent)
    {
        try
        {
            var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(errorContent);
            if (errorResponse?.TryGetValue("message", out var message) == true)
            {
                return message.ToString();
            }
        }
        catch
        {
            // Ignore JSON parsing errors
        }
        
        return null;
    }

    private async Task<string?> GetFromStorageAsync(string key)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
        }
        catch
        {
            return null;
        }
    }

    private async Task SetStorageAsync(string key, string value)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
        }
        catch
        {
            // Ignore storage errors
        }
    }

    private async Task RemoveFromStorageAsync(string key)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
        catch
        {
            // Ignore storage errors
        }
    }

    private void NotifyAuthenticationStateChanged()
    {
        AuthenticationStateChanged?.Invoke(_currentState);
    }
} 