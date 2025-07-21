using System.Security.Claims;
using CleverDocs.Core.Abstractions.Workspaces;
using CleverDocs.Core.Models.Workspaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CleverDocs.WebApi.Modules.Workspaces.Endpoints;

public static class InvitationEndpoints
{
    public static void MapInvitationEndpoints(this RouteGroupBuilder group)
    {
        // POST /api/workspaces/{workspaceId}/invitations
        group.MapPost("/{workspaceId:guid}/invitations", InviteUserHandler)
            .WithSummary("Invite user to workspace")
            .WithDescription("Send an invitation to a user (existing or new) to join the workspace")
            .Produces<WorkspaceInvitationDto>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        // GET /api/workspaces/{workspaceId}/invitations
        group.MapGet("/{workspaceId:guid}/invitations", GetPendingInvitationsHandler)
            .WithSummary("Get pending invitations")
            .WithDescription("Retrieve all pending invitations for a workspace")
            .Produces<List<WorkspaceInvitationDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        // POST /api/invitations/{token}/accept
        group.MapPost("/invitations/{token}/accept", AcceptInvitationHandler)
            .AllowAnonymous() // User might not be logged in yet
            .WithSummary("Accept workspace invitation")
            .WithDescription("Accept a workspace invitation using the invitation token")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        // DELETE /api/workspaces/{workspaceId}/invitations/{invitationId}
        group.MapDelete("/{workspaceId:guid}/invitations/{invitationId:guid}", CancelInvitationHandler)
            .WithSummary("Cancel invitation")
            .WithDescription("Cancel a pending workspace invitation")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> InviteUserHandler(
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

        var result = await workspaceService.InviteUserAsync(workspaceId, request, userId.Value);
        return result.IsSuccess 
            ? Results.Created($"/api/workspaces/{workspaceId}/invitations", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> GetPendingInvitationsHandler(
        Guid workspaceId,
        IWorkspaceService workspaceService,
        ClaimsPrincipal user)
    {
        var userId = GetUserIdFromClaims(user);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        // Check if user can manage members (and thus view invitations)
        var canManage = await workspaceService.CanUserManageMembersAsync(workspaceId, userId.Value);
        if (!canManage.IsSuccess || !canManage.Value)
        {
            return Results.Forbid();
        }

        var result = await workspaceService.GetPendingInvitationsAsync(workspaceId);
        return result.IsSuccess 
            ? Results.Ok(result.Value)
            : Results.NotFound(result.Error);
    }

    private static async Task<IResult> AcceptInvitationHandler(
        string token,
        IWorkspaceService workspaceService,
        ClaimsPrincipal user)
    {
        var userId = GetUserIdFromClaims(user);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        var result = await workspaceService.AcceptInvitationAsync(token, userId.Value);
        return result.IsSuccess 
            ? Results.Ok(new { Message = "Invitation accepted successfully" })
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CancelInvitationHandler(
        Guid workspaceId,
        Guid invitationId,
        IWorkspaceService workspaceService,
        ClaimsPrincipal user)
    {
        var userId = GetUserIdFromClaims(user);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        var result = await workspaceService.CancelInvitationAsync(invitationId, userId.Value);
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