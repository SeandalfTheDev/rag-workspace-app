using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using CleverDocs.Application.Auth.DTOs;
using CleverDocs.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CleverDocs.IntegrationTests.Auth;

public class RegisterEndpointTests(CleverDocsWebApplicationFactory applicationFactory): IntegrationTestFixture(applicationFactory)
{
    private readonly HttpClient _httpClient = applicationFactory.CreateClient();
    private readonly string _route = "/api/auth/register";
    private readonly IServiceProvider _serviceProvider = applicationFactory.Services;

    [Fact]
    public async Task GivenValidRequest_WhenRegisteringUser_ThenOkIsReturned()
    {
        await CleanupDatabaseAsync();
        var request = new RegisterUserDto()
        {
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "P@ssw0rd123",
            ConfirmPassword = "P@ssw0rd123"
        };

        var response = await _httpClient.PostAsJsonAsync(_route, request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    async Task GivenInUseEmail_WhenRegisteringUser_ThenConflictIsReturned()
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
        var request = new RegisterUserDto()
        {
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "P@ssw0rd123",
            ConfirmPassword = "P@ssw0rd123"
        };

        var response = await _httpClient.PostAsJsonAsync(_route, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GivenInvalidRequest_WhenRegisteringUser_ThenBadRequestIsReturned()
    {
        var request = new RegisterUserDto()
        {
            Email = "",
            FirstName = "",
            LastName = "",
            Password = "",
            ConfirmPassword = ""
        };

        var response = await _httpClient.PostAsJsonAsync(_route, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}