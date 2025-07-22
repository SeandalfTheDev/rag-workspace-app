namespace CleverDocs.Domain.Users;

public class UserRole
{
    public Guid UserId { get; set; }
    public int RoleId { get; set; }
    public Guid? ScopeId { get; set; }
    public string? ScopeType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}