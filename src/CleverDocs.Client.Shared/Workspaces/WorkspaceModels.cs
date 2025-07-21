using System.ComponentModel.DataAnnotations;
using CleverDocs.Client.Shared.Enums;

namespace CleverDocs.Client.Shared.Workspaces;

public record AddMemberRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }
    
    [Required]
    public required WorkspaceRole Role { get; init; } = WorkspaceRole.Member;
    
    public string? InvitationMessage { get; init; }
}

public record RemoveMemberRequest
{
    [Required]
    public required Guid UserId { get; init; }
}

public record UpdateMemberRoleRequest
{
    [Required]
    public required Guid UserId { get; init; }
    
    [Required]
    public required WorkspaceRole Role { get; init; }
}

public record WorkspaceMemberDto
{
    public required Guid UserId { get; init; }
    public required Guid WorkspaceId { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required WorkspaceRole Role { get; init; }
    public required DateTime JoinedAt { get; init; }
}

public record WorkspaceInvitationDto
{
    public required Guid Id { get; init; }
    public required Guid WorkspaceId { get; init; }
    public required string WorkspaceName { get; init; }
    public required string Email { get; init; }
    public required WorkspaceRole Role { get; init; }
    public required string InvitedByName { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public required bool IsAccepted { get; init; }
    public string? InvitationMessage { get; init; }
}

public record WorkspaceDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
} 