using System.Net;
using System.Net.Http.Json;
using CleverDocs.Infrastructure.Data;
using CleverDocs.Integration.Tests.Helpers;
using CleverDocs.Shared.Authentication.Models;
using DotNet.Testcontainers.Builders;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

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

    using var assertionScope = new AssertionScope();

    var responseBody = await response.Content.ReadFromJsonAsync<AuthResponse>();

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    responseBody.Should().NotBeNull();
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

    using var assertionScope = new AssertionScope();
    
    var responseBody = await response.Content.ReadFromJsonAsync<AuthResponse>();

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    responseBody.Should().NotBeNull();
    responseBody.Token.Should().NotBeNull();
    responseBody.RefreshToken.Should().NotBeNull();
    responseBody.User.Should().NotBeNull();
    responseBody.User.Email.Should().Be("test2@test.com");
  }
}