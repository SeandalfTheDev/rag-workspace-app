namespace CleverDocs.Domain.Auth;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>
    /// We'll use this to store the IdentityId from the Identity Provider.
    /// This could be any identity provider like Azure AD, Cognito, Keycloak, Auth0, etc.
    /// </summary>
    public string IdentityId { get; set; } = string.Empty;

    public static string NewId() => $"u_{Guid.CreateVersion7()}";
}