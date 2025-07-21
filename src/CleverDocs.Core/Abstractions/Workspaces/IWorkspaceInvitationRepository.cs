using CleverDocs.Core.Entities;

namespace CleverDocs.Core.Abstractions.Workspaces;

public interface IWorkspaceInvitationRepository
{
    Task<WorkspaceInvitation?> GetInvitationByIdAsync(Guid invitationId);
    Task<WorkspaceInvitation?> GetInvitationByTokenAsync(string token);
    Task<List<WorkspaceInvitation>> GetPendingInvitationsByWorkspaceIdAsync(Guid workspaceId);
    Task<List<WorkspaceInvitation>> GetPendingInvitationsByEmailAsync(string email);
    Task<WorkspaceInvitation?> GetPendingInvitationAsync(Guid workspaceId, string email);
    Task<WorkspaceInvitation> CreateInvitationAsync(WorkspaceInvitation invitation);
    Task<WorkspaceInvitation> UpdateInvitationAsync(WorkspaceInvitation invitation);
    Task<bool> DeleteInvitationAsync(Guid invitationId);
    Task<bool> HasPendingInvitationAsync(Guid workspaceId, string email);
} 