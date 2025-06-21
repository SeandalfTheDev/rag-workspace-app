using System.Text;
using CleverDocs.Core.Abstractions.Authentication;
using CleverDocs.Core.Abstractions.Documents;
using CleverDocs.Core.Abstractions.Repositories;
using CleverDocs.Core.Configuration;
using CleverDocs.Core.Validation.Authentication;
using CleverDocs.Infrastructure.Authentication;
using CleverDocs.Infrastructure.Data;
using CleverDocs.Infrastructure.Data.Repositories;
using CleverDocs.Infrastructure.Documents;
using CleverDocs.Infrastructure.TextProcessing;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CleverDocs.IoC;

public static class ServiceExtensions
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

    return services;
  }

  public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
  {
    var jwtSettings = new JwtSettings
    {
      SecretKey = configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JwtSettings:SecretKey is not set"),
      Issuer = configuration["JwtSettings:Issuer"] ?? throw new InvalidOperationException("JwtSettings:Issuer is not set"),
      Audience = configuration["JwtSettings:Audience"] ?? throw new InvalidOperationException("JwtSettings:Audience is not set"),
      ExpirationHours = int.Parse(configuration["JwtSettings:ExpirationHours"] ?? throw new InvalidOperationException("JwtSettings:ExpirationHours is not set")),
      RefreshTokenExpirationDays = int.Parse(configuration["JwtSettings:RefreshTokenExpirationDays"] ?? throw new InvalidOperationException("JwtSettings:RefreshTokenExpirationDays is not set")),
    };

    services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

    var tokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        RequireExpirationTime = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    services.AddSingleton(tokenValidationParameters);

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = tokenValidationParameters;
    });

    services.AddAuthorization();

    services.AddScoped<ITokenService, TokenService>();
    services.AddScoped<IPasswordService, PasswordService>();
    services.AddScoped<IAuthService, AuthService>();

    return services;
  }

  public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
  {
    var connectionString = configuration.GetConnectionString("Postgres") ?? throw new InvalidOperationException("Connection string 'Postgres' is not set");

    services.AddDbContext<ApplicationDbContext>(options =>
    {
      options.UseNpgsql(connectionString, opt => opt.UseVector());
      options.UseSnakeCaseNamingConvention();
    });
    return services;
  }

  public static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IDocumentRepository, DocumentRepository>();
    services.AddScoped<IDocumentChunkRepository, DocumentChunkRepository>();
    
    return services;
  }

  public static IServiceCollection AddDocumentServices(this IServiceCollection services)
  {
    services.AddScoped<ITextExtractionService, TextExtractionService>();
    services.AddScoped<IChunkingService, ChunkingService>();
    services.AddScoped<IEmbeddingService, EmbeddingService>();
    services.AddScoped<IDocumentProcessingService, DocumentProcessingService>();
    
    return services;
  }

  public static IServiceCollection AddQueueServices(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddHostedService<QueuedHostedService>();
    services.AddScoped<IBackgroundTaskQueue, BackgroundTaskQueue>();
    
    return services;
  }
}