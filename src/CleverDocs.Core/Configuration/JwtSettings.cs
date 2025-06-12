namespace CleverDocs.Core.Configuration;

public class JwtSettings
{
    public static string SectionName => "JwtSettings";
    public required string SecretKey { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required int ExpirationHours { get; init; }
    public required int RefreshTokenExpirationDays { get; init; }
}