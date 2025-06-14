using System.Net;
using System.Net.Http.Json;
using CleverDocs.Core.Entities;
using CleverDocs.Infrastructure.Data;
using CleverDocs.Integration.Tests.Helpers;
using CleverDocs.Shared.Authentication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace CleverDocs.Integration.Tests.Authentication;

public class LoginEndpointTests : IClassFixture<WebApiTestFactory>
{
    private readonly HttpClient _client;

    public LoginEndpointTests(WebApiTestFactory webApiTestFactory)
    {
        _client = webApiTestFactory.CreateClient();
    }
    
    [Fact]
    public async Task Login_Returns_200_OK()    
    {
        await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
        {
            Email = "test2@test.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
        });
        
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = "test2@test.com",
            Password = "Password123!"
        });
        
        var responseBody = await response.Content.ReadFromJsonAsync<AuthResponse>();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        responseBody.ShouldNotBeNull();
        responseBody.Token.ShouldNotBeNull();
        responseBody.RefreshToken.ShouldNotBeNull();
        responseBody.User.ShouldNotBeNull();
        responseBody.User.Email.ShouldBe("test2@test.com");
    }
    
    [Fact]
    public async Task LoginHandler_Returns_400_With_ValidationProblemDetails_When_Email_Is_Invalid()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = "invalid-email", // Invalid email format
            Password = "Password123!"
        });

        var responseBody = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        var hasEmailError = responseBody?.Errors.ContainsKey("Email");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        responseBody.ShouldNotBeNull();
        responseBody.Errors.ShouldNotBeNull();
        responseBody.Errors.Count.ShouldBe(1);
        hasEmailError.ShouldBe(true);
    }
    
    [Fact]
    public async Task LoginHandler_Returns_400_With_ValidationProblemDetails_When_Password_Is_Empty()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = "test@test.com", // Invalid email format
            Password = "" // Weak password
        });

        var responseBody = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        var hasPasswordError = responseBody?.Errors.ContainsKey("Password");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        responseBody.ShouldNotBeNull();
        responseBody.Errors.ShouldNotBeNull();
        responseBody.Errors.Count.ShouldBe(1);
        hasPasswordError.ShouldBe(true);
    }
}