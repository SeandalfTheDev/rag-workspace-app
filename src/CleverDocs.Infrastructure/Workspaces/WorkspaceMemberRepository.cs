using CleverDocs.Core.Abstractions.Workspaces;
using CleverDocs.Core.Entities;
using CleverDocs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleverDocs.Infrastructure.Workspaces;

public class WorkspaceMemberRepository(ApplicationDbContext context) : IWorkspaceMemberRepository
{
    public async Task<WorkspaceMember?> CreateWorkspaceMemberAsync(WorkspaceMember workspaceMember)
    {
        context.WorkspaceMembers.Add(workspaceMember);
        await context.SaveChangesAsync();
        return workspaceMember;
    }

    public async Task<WorkspaceMember?> DeleteWorkspaceMemberAsync(Guid workspaceMemberId)
    {
        var workspaceMember = await context.WorkspaceMembers.FindAsync(workspaceMemberId);
        if (workspaceMember == null)
        {
            return null;
        }
        context.WorkspaceMembers.Remove(workspaceMember);
        await context.SaveChangesAsync();
        return workspaceMember;
    }

    public async Task<WorkspaceMember?> GetWorkspaceMemberByIdAsync(Guid userId, Guid workspaceId)
    {
        return await context.WorkspaceMembers
          .SingleOrDefaultAsync(wm => wm.UserId == userId && wm.WorkspaceId == workspaceId);
    }

    public async Task<List<WorkspaceMember>> GetWorkspaceMembersByWorkspaceIdAsync(Guid workspaceId)
    {
        return await context.WorkspaceMembers
          .Where(wm => wm.WorkspaceId == workspaceId)
          .ToListAsync();
    }

    public async Task<WorkspaceMember?> UpdateWorkspaceMemberAsync(WorkspaceMember workspaceMember)
    {
        var existingWorkspaceMember = await context.WorkspaceMembers
          .SingleOrDefaultAsync(wm => wm.UserId == workspaceMember.UserId && wm.WorkspaceId == workspaceMember.WorkspaceId);
        if (existingWorkspaceMember == null)
        {
            return null;
        }

        context.WorkspaceMembers.Update(workspaceMember);
        await context.SaveChangesAsync();
        return workspaceMember;
    }
}