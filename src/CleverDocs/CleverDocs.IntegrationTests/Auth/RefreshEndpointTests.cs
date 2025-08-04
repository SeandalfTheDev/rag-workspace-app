using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using CleverDocs.Application.Auth.DTOs;
using CleverDocs.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CleverDocs.IntegrationTests.Auth;

public class RefreshEndpointTests(CleverDocsWebApplicationFactory applicationFactory) : IntegrationTestFixture(applicationFactory)
{
    private readonly HttpClient _httpClient = applicationFactory.CreateClient();
    private readonly string _route = "/api/auth/refresh";
    private readonly IServiceProvider _serviceProvider = applicationFactory.Services;
    
    [Fact]
    public async Task GivenValidRequest_WhenRefreshingToken_ThenOkIsReturned()
    {
        await CleanupDatabaseAsync();
        
        // Arrange
        var registerRequest = new RegisterUserDto()
        {
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "P@ssw0rd123",
            ConfirmPassword = "P@ssw0rd123"
        };

        var registerResponse = await _httpClient.PostAsJsonAsync("api/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();
        
        var response = await registerResponse.Content.ReadFromJsonAsync<AccessTokensDto>();

        // Act
        var refreshRequest = new RefreshTokenDto()
        {
            RefreshToken = response.RefreshToken
        };
        
        var refreshResponse = await _httpClient.PostAsJsonAsync(_route, refreshRequest);
        refreshResponse.EnsureSuccessStatusCode();
        var refreshResult = await refreshResponse.Content.ReadFromJsonAsync<AccessTokensDto>();
        
        // Assert
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        refreshResult.Should().NotBeNull();
        refreshResult.AccessToken.Should().NotBeNullOrEmpty();
        refreshResult.RefreshToken.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public async Task GivenInvalidRequest_WhenRefreshingToken_ThenBadRequestIsReturned()
    {
        var request = new RefreshTokenDto()
        {
            RefreshToken = ""
        };
        
        var response = await _httpClient.PostAsJsonAsync(_route, request);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task GivenInvalidRefreshToken_WhenRefreshingToken_ThenUnauthorizedIsReturned()
    {
        var request = new RefreshTokenDto()
        {
            RefreshToken = "InvalidRefreshToken"
        };
        
        var response = await _httpClient.PostAsJsonAsync(_route, request);
        
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}