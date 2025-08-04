using System.Net.Http.Headers;
using System.Net.Http.Json;
using CleverDocs.Application.Auth.DTOs;
using CleverDocs.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace CleverDocs.IntegrationTests.Infrastructure;

public class IntegrationTestFixture(CleverDocsWebApplicationFactory factory) : IClassFixture<CleverDocsWebApplicationFactory>
{
    private HttpClient? _authorizedClient;
    
    public HttpClient CreateClient() => factory.CreateClient();
    
    protected async Task CleanupDatabaseAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();
        IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var connectionString = factory.ConnectionString;
        if (connectionString is null)
        {
            throw new InvalidOperationException("Database connection string not found in configuration");
        }

        await using NpgsqlConnection connection = new(connectionString);
        await connection.OpenAsync();

        await using NpgsqlCommand command = new(@"
            DO $$
            BEGIN
                -- Truncate application tables
                TRUNCATE TABLE clever_docs.users CASCADE;

                -- Truncate identity tables
                TRUNCATE TABLE identity.asp_net_users CASCADE;
                TRUNCATE TABLE identity.refresh_tokens CASCADE;
            END $$;", connection);

        await command.ExecuteNonQueryAsync();
    }
    
    public async Task<HttpClient> CreateAuthenticatedClientAsync(
        string email = "test@test.com",
        string password = "Test123!",
        bool forceNewClient = false)
    {
        if (_authorizedClient is not null && !forceNewClient)
        {
            return _authorizedClient;
        }

        var client = CreateClient();

        // Check if a user exists
        bool userExists;
        using (IServiceScope scope = factory.Services.CreateScope())
        {
            using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            userExists = await dbContext.Users.AnyAsync(u => u.Email == email);
        }

        if (!userExists)
        {
            // Register a new user
            var registerResponse = await client.PostAsJsonAsync("/api/auth/register",
                new RegisterUserDto
                {
                    Email = email,
                    FirstName = email,
                    LastName = email,
                    Password = password,
                    ConfirmPassword = password
                });

            registerResponse.EnsureSuccessStatusCode();
        }

        // Login to get the token
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login",
            new LoginUserDto
            {
                Email = email,
                Password = password
            });

        loginResponse.EnsureSuccessStatusCode();

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AccessTokensDto>();

        if (loginResult?.AccessToken is null)
        {
            throw new InvalidOperationException("Failed to get authentication token");
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);
        if (!forceNewClient)
        {
            _authorizedClient = client;
        }

        return client;
    }
}