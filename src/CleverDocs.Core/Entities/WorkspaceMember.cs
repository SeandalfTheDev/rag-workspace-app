using CleverDocs.Shared.Enums;

namespace CleverDocs.Core.Entities;

public class WorkspaceMember
{
  public Guid WorkspaceId { get; set;}
  public Guid UserId { get; set;}
  public WorkspaceRole Role { get; set;} = WorkspaceRole.Member;
  public DateTime JoinedAt { get; set;} = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set;} = DateTime.UtcNow;
}