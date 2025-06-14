using System.Net;
using System.Net.Http.Json;
using CleverDocs.Integration.Tests.Helpers;
using CleverDocs.Shared.Authentication.Models;
using Microsoft.AspNetCore.Mvc;
using Shouldly;


namespace CleverDocs.Integration.Tests.Authentication;

public class RegisterEndpointTests : IClassFixture<WebApiTestFactory>
{
  private readonly HttpClient _client;

  public RegisterEndpointTests(WebApiTestFactory webApiTestFactory)
  {
    _client = webApiTestFactory.CreateClient();
  }

  [Fact]
  public async Task Register_Returns_200_OK()
  {
    var response = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
    {
      Email = "test@test.com",
      Password = "Password123!",
      FirstName = "John",
      LastName = "Doe",
    });

    var responseBody = await response.Content.ReadFromJsonAsync<AuthResponse>();

    response.StatusCode.ShouldBe(HttpStatusCode.OK);

  }

  [Fact]
  public async Task RegisterHandler_Returns_200_With_Valid_AuthResponse()
  {
    var response = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
    {
      Email = "test2@test.com", // Use different email to avoid conflicts
      Password = "Password123!",
      FirstName = "John",
      LastName = "Doe",
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
  public async Task RegisterHandler_Returns_400_With_ValidationProblemDetails_When_Email_Is_Invalid()
  {
    var response = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
    {
      Email = "invalid-email", // Invalid email format
      Password = "Password123!",
      FirstName = "John",
      LastName = "Doe",
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
  public async Task RegisterHandler_Returns_400_With_ValidationProblemDetails_When_Password_Is_Weak()
  {
    var response = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
    {
      Email = "invalid-email", // Invalid email format
      Password = "apple", // Weak password
      FirstName = "John",
      LastName = "Doe",
    });

    var responseBody = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
    var hasPasswordError = responseBody?.Errors.ContainsKey("Password");

    response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    responseBody.ShouldNotBeNull();
    responseBody.Errors.ShouldNotBeNull();
    responseBody.Errors.Count.ShouldBe(2);
    hasPasswordError.ShouldBe(true);
  }
  
  [Fact]
  public async Task RegisterHandler_Returns_400_With_ValidationProblemDetails_When_FirstName_Is_Empty()
  {
    var response = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
    {
      Email = "invalid-email", // Invalid email format
      Password = "password", // Weak password
      FirstName = "",
      LastName = "Doe",
    });

    var responseBody = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
    var hasFirstNameError = responseBody?.Errors.ContainsKey("FirstName");

    response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    responseBody.ShouldNotBeNull();
    responseBody.Errors.ShouldNotBeNull();
    responseBody.Errors.Count.ShouldBe(2);
    hasFirstNameError.ShouldBe(true);
  }
  
  [Fact]
  public async Task RegisterHandler_Returns_400_With_ValidationProblemDetails_When_LastName_Is_Empty()
  {
    var response = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
    {
      Email = "invalid-email", // Invalid email format
      Password = "password", // Weak password
      FirstName = "John",
      LastName = "",
    });

    var responseBody = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
    var hasLastNameError = responseBody?.Errors.ContainsKey("LastName");

    response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    responseBody.ShouldNotBeNull();
    responseBody.Errors.ShouldNotBeNull();
    responseBody.Errors.Count.ShouldBe(2);
    hasLastNameError.ShouldBe(true);
  }
}