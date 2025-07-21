using System.Security.Claims;
using CleverDocs.Core.Abstractions.Workspaces;
using CleverDocs.Core.Models.Workspaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CleverDocs.WebApi.Modules.Workspaces.Endpoints;

public static class MemberManagementEndpoints
{
    public static void MapMemberManagementEndpoints(this RouteGroupBuilder group)
    {
        // GET /api/workspaces/{workspaceId}/members
        group.MapGet("/{workspaceId:guid}/members", GetWorkspaceMembersHandler)
            .WithSummary("Get all workspace members")
            .WithDescription("Retrieve all members of a specific workspace")
            .Produces<List<WorkspaceMemberDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden);

        // POST /api/workspaces/{workspaceId}/members
        group.MapPost("/{workspaceId:guid}/members", AddMemberHandler)
            .WithSummary("Add a member to workspace")
            .WithDescription("Add an existing user directly to the workspace")
            .Produces<WorkspaceMemberDto>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        // PUT /api/workspaces/{workspaceId}/members/{userId}/role
        group.MapPut("/{workspaceId:guid}/members/{userId:guid}/role", UpdateMemberRoleHandler)
            .WithSummary("Update member role")
            .WithDescription("Change the role of an existing workspace member")
            .Produces<WorkspaceMemberDto>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        // DELETE /api/workspaces/{workspaceId}/members/{userId}
        group.MapDelete("/{workspaceId:guid}/members/{userId:guid}", RemoveMemberHandler)
            .WithSummary("Remove member from workspace")
            .WithDescription("Remove a member from the workspace")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetWorkspaceMembersHandler(
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

        var result = await workspaceService.GetWorkspaceMembersAsync(workspaceId);
        return result.IsSuccess 
            ? Results.Ok(result.Value)
            : Results.NotFound(result.Error);
    }

    private static async Task<IResult> AddMemberHandler(
        Guid workspaceId,
        AddMemberRequest request,
        IWorkspaceService workspaceService,
        IValidator<AddMemberRequest> validator,
        ClaimsPrincipal user)
    {
        var userId = GetUserIdFromClaims(user);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var result = await workspaceService.AddMemberAsync(workspaceId, request, userId.Value);
        return result.IsSuccess 
            ? Results.Created($"/api/workspaces/{workspaceId}/members", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateMemberRoleHandler(
        Guid workspaceId,
        Guid userId,
        UpdateMemberRoleRequest request,
        IWorkspaceService workspaceService,
        IValidator<UpdateMemberRoleRequest> validator,
        ClaimsPrincipal user)
    {
        var currentUserId = GetUserIdFromClaims(user);
        if (currentUserId == null)
        {
            return Results.Unauthorized();
        }

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var result = await workspaceService.UpdateWorkspaceMemberAsync(workspaceId, userId, request.Role, currentUserId.Value);
        return result.IsSuccess 
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> RemoveMemberHandler(
        Guid workspaceId,
        Guid userId,
        IWorkspaceService workspaceService,
        ClaimsPrincipal user)
    {
        var currentUserId = GetUserIdFromClaims(user);
        if (currentUserId == null)
        {
            return Results.Unauthorized();
        }

        var result = await workspaceService.RemoveWorkspaceMemberAsync(workspaceId, userId, currentUserId.Value);
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