using CleverDocs.WebApi.Modules.Workspaces.Endpoints;

namespace CleverDocs.WebApi.Modules.Workspaces;

public static class WorkspaceModule
{
    public static void MapWorkspaceEndpoints(this IEndpointRouteBuilder app)
    {
        var workspaceGroup = app.MapGroup("/api/workspaces")
            .WithTags("Workspaces")
            .RequireAuthorization();

        // Workspace management
        workspaceGroup.MapWorkspaceManagementEndpoints();
        
        // Member management  
        workspaceGroup.MapMemberManagementEndpoints();
        
        // Invitation management
        workspaceGroup.MapInvitationEndpoints();
    }
} 