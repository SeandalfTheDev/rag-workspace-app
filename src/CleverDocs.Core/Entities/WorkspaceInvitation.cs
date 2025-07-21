using CleverDocs.Shared.Enums;

namespace CleverDocs.Core.Entities;

public class WorkspaceInvitation
{
    public Guid Id { get; set; }
    public Guid WorkspaceId { get; set; }
    public string Email { get; set; } = string.Empty;
    public WorkspaceRole Role { get; set; } = WorkspaceRole.Member;
    public Guid InvitedBy { get; set; }
    public string InvitationToken { get; set; } = string.Empty;
    public string? InvitationMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7); // 7 days to accept
    public bool IsAccepted { get; set; } = false;
    public DateTime? AcceptedAt { get; set; }
    
    // Navigation properties
    public virtual Workspace Workspace { get; set; } = null!;
    public virtual User InvitedByUser { get; set; } = null!;
} 