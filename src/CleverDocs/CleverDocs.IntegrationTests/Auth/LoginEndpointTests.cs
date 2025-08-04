using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using CleverDocs.Application.Auth.DTOs;
using CleverDocs.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CleverDocs.IntegrationTests.Auth;

public class LoginEndpointTests(CleverDocsWebApplicationFactory applicationFactory) : IntegrationTestFixture(applicationFactory)
{
    private readonly HttpClient _httpClient = applicationFactory.CreateClient();
    private readonly string _route = "/api/auth/login";
    private readonly IServiceProvider _serviceProvider = applicationFactory.Services;
    
    [Fact]
    public async Task GivenValidCredentials_WhenLogin_ThenReturnsToken()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        
        var identityUser = new IdentityUser
        {
            Email = "john.doe@example.com",
            UserName = "john.doe@example.com"
        };

        await userManager.CreateAsync(identityUser, "P@ssw0rd123");
        
        // Act
        var request = new LoginUserDto()
        {
            Email = identityUser.Email,
            Password = "P@ssw0rd123"
        };
        
        var response = await _httpClient.PostAsJsonAsync(_route, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GivenInvalidRequest_WhenLogin_ThenBadRequestIsReturned()
    {
        var request = new LoginUserDto()
        {
            Email = "",
            Password = ""
        };
        
        var response = await _httpClient.PostAsJsonAsync(_route, request);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GivenEmailDoesNotExist_WhenLogin_ThenConflictIsReturned()
    {
        await CleanupDatabaseAsync();

        var request = new LoginUserDto()
        {
            Email = "john.doe@example.com",
            Password = "P@ssw0rd123"
        };
        
        var response = await _httpClient.PostAsJsonAsync(_route, request);
        
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}