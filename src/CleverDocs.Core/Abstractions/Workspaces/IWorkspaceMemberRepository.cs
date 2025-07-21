using CleverDocs.Core.Entities;

namespace CleverDocs.Core.Abstractions.Workspaces;

public interface IWorkspaceMemberRepository
{
  Task<WorkspaceMember?> GetWorkspaceMemberByIdAsync(Guid userId, Guid workspaceId);
  Task<List<WorkspaceMember>> GetWorkspaceMembersByWorkspaceIdAsync(Guid workspaceId);
  Task<WorkspaceMember?> CreateWorkspaceMemberAsync(WorkspaceMember workspaceMember);
  Task<WorkspaceMember?> UpdateWorkspaceMemberAsync(WorkspaceMember workspaceMember);
  Task<WorkspaceMember?> DeleteWorkspaceMemberAsync(Guid workspaceMemberId);
}