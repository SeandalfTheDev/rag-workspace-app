using CleverDocs.Core.Entities;

namespace CleverDocs.Core.Abstractions.Workspaces;

public interface IWorkspaceRepository
{
  Task<Workspace?> GetWorkspaceByIdAsync(Guid workspaceId);
  Task<List<Workspace>> GetWorkspacesByUserIdAsync(Guid userId);
  Task<List<Workspace>> GetWorkspaceOwnedByUserIdAsync(Guid userId);
  Task<List<Workspace>> GetWorkspaceUserIsMemberOfAsync(Guid userId);
  Task<Workspace?> CreateWorkspaceAsync(Workspace workspace);
  Task<Workspace?> UpdateWorkspaceAsync(Workspace workspace);
  Task<Workspace?> DeleteWorkspaceAsync(Guid workspaceId);
}
