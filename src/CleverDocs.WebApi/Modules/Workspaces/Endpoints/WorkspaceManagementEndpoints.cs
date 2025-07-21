using System.Security.Claims;
using CleverDocs.Core.Abstractions.Workspaces;
using CleverDocs.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleverDocs.WebApi.Modules.Workspaces.Endpoints;

public static class WorkspaceManagementEndpoints
{
    public static void MapWorkspaceManagementEndpoints(this RouteGroupBuilder group)
    {
        // GET /api/workspaces
        group.MapGet("/", GetUserWorkspacesHandler)
            .WithSummary("Get user workspaces")
            .WithDescription("Retrieve all workspaces for the current user")
            .Produces<List<Workspace>>(StatusCodes.Status200OK);

        // GET /api/workspaces/{workspaceId}
        group.MapGet("/{workspaceId:guid}", GetWorkspaceHandler)
            .WithSummary("Get workspace by ID")
            .WithDescription("Retrieve a specific workspace by its ID")
            .Produces<Workspace>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden);

        // POST /api/workspaces
        group.MapPost("/", CreateWorkspaceHandler)
            .WithSummary("Create workspace")
            .WithDescription("Create a new workspace")
            .Produces<Workspace>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest);

        // PUT /api/workspaces/{workspaceId}
        group.MapPut("/{workspaceId:guid}", UpdateWorkspaceHandler)
            .WithSummary("Update workspace")
            .WithDescription("Update an existing workspace")
            .Produces<Workspace>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden);

        // DELETE /api/workspaces/{workspaceId}
        group.MapDelete("/{workspaceId:guid}", DeleteWorkspaceHandler)
            .WithSummary("Delete workspace")
            .WithDescription("Delete a workspace (only owners can do this)")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetUserWorkspacesHandler(
        IWorkspaceService workspaceService,
        ClaimsPrincipal user)
    {
        var userId = GetUserIdFromClaims(user);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        var result = await workspaceService.GetWorkspacesByUserIdAsync(userId.Value);
        return result.IsSuccess 
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> GetWorkspaceHandler(
        Guid workspaceId,
        IWorkspaceService workspaceService,
        ClaimsPrincipal user)
    {
        var userId = GetUserIdFromClaims(user);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        // Check if user has access to this workspace
        var canAccess = await workspaceService.CanUserManageMembersAsync(workspaceId, userId.Value);
        if (!canAccess.IsSuccess || !canAccess.Value)
        {
            return Results.Forbid();
        }

        var result = await workspaceService.GetWorkspaceByIdAsync(workspaceId);
        return result.IsSuccess 
            ? Results.Ok(result.Value)
            : Results.NotFound(result.Error);
    }

    private static async Task<IResult> CreateWorkspaceHandler(
        CreateWorkspaceRequest request,
        IWorkspaceService workspaceService,
        ClaimsPrincipal user)
    {
        var userId = GetUserIdFromClaims(user);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        var workspace = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await workspaceService.CreateWorkspaceAsync(workspace);
        return result.IsSuccess 
            ? Results.Created($"/api/workspaces/{workspace.Id}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateWorkspaceHandler(
        Guid workspaceId,
        UpdateWorkspaceRequest request,
        IWorkspaceService workspaceService,
        ClaimsPrincipal user)
    {
        var userId = GetUserIdFromClaims(user);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        // Check if user can manage this workspace
        var canManage = await workspaceService.CanUserManageMembersAsync(workspaceId, userId.Value);
        if (!canManage.IsSuccess || !canManage.Value)
        {
            return Results.Forbid();
        }

        // Get existing workspace
        var existingResult = await workspaceService.GetWorkspaceByIdAsync(workspaceId);
        if (!existingResult.IsSuccess)
        {
            return Results.NotFound(existingResult.Error);
        }

        var workspace = existingResult.Value;
        workspace.Name = request.Name;
        workspace.Description = request.Description ?? string.Empty;
        workspace.UpdatedAt = DateTime.UtcNow;

        var result = await workspaceService.UpdateWorkspaceAsync(workspace);
        return result.IsSuccess 
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> DeleteWorkspaceHandler(
        Guid workspaceId,
        IWorkspaceService workspaceService,
        ClaimsPrincipal user)
    {
        var userId = GetUserIdFromClaims(user);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        // TODO: Check if user is owner of this workspace
        // For now, just check if they can manage members
        var canManage = await workspaceService.CanUserManageMembersAsync(workspaceId, userId.Value);
        if (!canManage.IsSuccess || !canManage.Value)
        {
            return Results.Forbid();
        }

        var result = await workspaceService.DeleteWorkspaceAsync(workspaceId);
        return result.IsSuccess 
            ? Results.NoContent()
            : Results.BadRequest(result.Error);
    }

    private static Guid? GetUserIdFromClaims(ClaimsPrincipal user)
    {
        var idClaim = user.FindFirst("id")?.Value;
        return Guid.TryParse(idClaim, out var userId) ? userId : null;
    }
}

// Request DTOs
public record CreateWorkspaceRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}

public record UpdateWorkspaceRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
} 