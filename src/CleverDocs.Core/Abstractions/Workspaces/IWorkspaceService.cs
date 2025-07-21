using CleverDocs.Core.Entities;
using CleverDocs.Core.Models.Common;
using CleverDocs.Core.Models.Workspaces;
using CleverDocs.Shared.Enums;

namespace CleverDocs.Core.Abstractions.Workspaces;

public interface IWorkspaceService
{
  // Workspace Management
  Task<AppResult<Workspace>> CreateWorkspaceAsync(Workspace workspace);
  Task<AppResult<Workspace>> DeleteWorkspaceAsync(Guid workspaceId);
  Task<AppResult<Workspace>> GetWorkspaceByIdAsync(Guid workspaceId);
  Task<AppResult<List<Workspace>>> GetWorkspacesByUserIdAsync(Guid userId);
  Task<AppResult<List<Workspace>>> GetWorkspaceUserIsMemberOfAsync(Guid userId);
  Task<AppResult<Workspace>> UpdateWorkspaceAsync(Workspace workspace);
  
  // Member Management  
  Task<AppResult<WorkspaceMemberDto>> AddMemberAsync(Guid workspaceId, AddMemberRequest request, Guid invitedBy);
  Task<AppResult<WorkspaceMember>> RemoveWorkspaceMemberAsync(Guid workspaceId, Guid userId, Guid removedBy);
  Task<AppResult<WorkspaceMember>> UpdateWorkspaceMemberAsync(Guid workspaceId, Guid userId, WorkspaceRole role, Guid updatedBy);
  Task<AppResult<List<WorkspaceMemberDto>>> GetWorkspaceMembersAsync(Guid workspaceId);
  
  // Invitation Management
  Task<AppResult<WorkspaceInvitationDto>> InviteUserAsync(Guid workspaceId, AddMemberRequest request, Guid invitedBy);
  Task<AppResult<WorkspaceMember>> AcceptInvitationAsync(string token, Guid userId);
  Task<AppResult<bool>> CancelInvitationAsync(Guid invitationId, Guid canceledBy);
  Task<AppResult<List<WorkspaceInvitationDto>>> GetPendingInvitationsAsync(Guid workspaceId);
  
  // Permission Validation
  Task<AppResult<bool>> CanUserManageMembersAsync(Guid workspaceId, Guid userId);
  Task<AppResult<bool>> CanUserChangeRoleAsync(Guid workspaceId, Guid userId, WorkspaceRole targetRole);
}