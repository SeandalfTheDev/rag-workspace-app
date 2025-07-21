using CleverDocs.Core.Abstractions.Workspaces;
using CleverDocs.Core.Entities;
using CleverDocs.Infrastructure.Data;
using CleverDocs.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace CleverDocs.Infrastructure.Workspaces;

public class WorkspaceRepository(ApplicationDbContext context) : IWorkspaceRepository
{
    public async Task<Workspace?> CreateWorkspaceAsync(Workspace workspace)
    {
        context.Workspaces.Add(workspace);
        await context.SaveChangesAsync();
        return workspace;
    }

    public async Task<Workspace?> DeleteWorkspaceAsync(Guid workspaceId)
    {
        var workspace = await context.Workspaces.FindAsync(workspaceId);
        if (workspace == null)
        {
            return null;
        }
        context.Workspaces.Remove(workspace);
        await context.SaveChangesAsync();
        return workspace;
    }

    public async Task<Workspace?> GetWorkspaceByIdAsync(Guid workspaceId)
    {
        return await context.Workspaces.FindAsync(workspaceId);
    }

    public async Task<List<Workspace>> GetWorkspaceOwnedByUserIdAsync(Guid userId)
    {
        var workspaces = await context.WorkspaceMembers
            .Include(wm => wm.Workspace)
            .Where(wm => wm.UserId == userId && wm.Role == WorkspaceRole.Owner)
            .Select(wm => wm.Workspace)
            .ToListAsync();

        return workspaces;
    }

    public async Task<List<Workspace>> GetWorkspacesByUserIdAsync(Guid userId)
    {
        var workspaces = await context.WorkspaceMembers
            .Include(wm => wm.Workspace)
            .Where(wm => wm.UserId == userId)
            .Select(wm => wm.Workspace)
            .ToListAsync();

        return workspaces;
    }

    public async Task<List<Workspace>> GetWorkspaceUserIsMemberOfAsync(Guid userId)
    {
        var workspaces = await context.WorkspaceMembers
            .Where(wm => wm.UserId == userId && wm.Role == WorkspaceRole.Member)
            .Select(wm => wm.Workspace)
            .ToListAsync();

        return workspaces;
    }

    public async Task<Workspace?> UpdateWorkspaceAsync(Workspace workspace)
    {
        var existingWorkspace = await context.Workspaces.FindAsync(workspace.Id);
        if (existingWorkspace == null)
        {
            return null;
        }

        context.Workspaces.Update(workspace);
        await context.SaveChangesAsync();
        return workspace;
    }
}