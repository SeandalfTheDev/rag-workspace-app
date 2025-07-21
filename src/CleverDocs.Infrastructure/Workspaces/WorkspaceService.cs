using System.Security.Cryptography;
using CleverDocs.Core.Abstractions.Authentication;
using CleverDocs.Core.Abstractions.Repositories;
using CleverDocs.Core.Abstractions.Workspaces;
using CleverDocs.Core.Entities;
using CleverDocs.Core.Models.Common;
using CleverDocs.Core.Models.Workspaces;
using CleverDocs.Shared.Enums;
using Microsoft.Extensions.Logging;

namespace CleverDocs.Infrastructure.Workspaces;

public class WorkspaceService : IWorkspaceService
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
    private readonly IWorkspaceInvitationRepository _invitationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<WorkspaceService> _logger;

    public WorkspaceService(
        IWorkspaceRepository workspaceRepository,
        IWorkspaceMemberRepository workspaceMemberRepository,
        IWorkspaceInvitationRepository invitationRepository,
        IUserRepository userRepository,
        ILogger<WorkspaceService> logger)
    {
        _workspaceRepository = workspaceRepository;
        _workspaceMemberRepository = workspaceMemberRepository;
        _invitationRepository = invitationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    #region Workspace Management
    
    public async Task<AppResult<Workspace>> CreateWorkspaceAsync(Workspace workspace)
    {
        try
        {
            var createdWorkspace = await _workspaceRepository.CreateWorkspaceAsync(workspace);
            return createdWorkspace != null 
                ? AppResult<Workspace>.Success(createdWorkspace)
                : AppResult<Workspace>.Failure(AppError.Generic("Failed to create workspace"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating workspace");
            return AppResult<Workspace>.Failure(AppError.Generic(ex.Message));
        }
    }

    public async Task<AppResult<Workspace>> DeleteWorkspaceAsync(Guid workspaceId)
    {
        try
        {
            var deletedWorkspace = await _workspaceRepository.DeleteWorkspaceAsync(workspaceId);
            return deletedWorkspace != null 
                ? AppResult<Workspace>.Success(deletedWorkspace)
                : AppResult<Workspace>.Failure(AppError.RecordNotFound("Workspace not found"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting workspace {WorkspaceId}", workspaceId);
            return AppResult<Workspace>.Failure(AppError.Generic(ex.Message));
        }
    }

    public async Task<AppResult<Workspace>> GetWorkspaceByIdAsync(Guid workspaceId)
    {
        try
        {
            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(workspaceId);
            return workspace != null 
                ? AppResult<Workspace>.Success(workspace)
                : AppResult<Workspace>.Failure(AppError.RecordNotFound("Workspace not found"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workspace {WorkspaceId}", workspaceId);
            return AppResult<Workspace>.Failure(AppError.Generic(ex.Message));
        }
    }

    public async Task<AppResult<List<Workspace>>> GetWorkspacesByUserIdAsync(Guid userId)
    {
        try
        {
            var workspaces = await _workspaceRepository.GetWorkspacesByUserIdAsync(userId);
            return AppResult<List<Workspace>>.Success(workspaces);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workspaces for user {UserId}", userId);
            return AppResult<List<Workspace>>.Failure(AppError.Generic(ex.Message));
        }
    }

    public async Task<AppResult<List<Workspace>>> GetWorkspaceUserIsMemberOfAsync(Guid userId)
    {
        try
        {
            var workspaces = await _workspaceRepository.GetWorkspaceUserIsMemberOfAsync(userId);
            return AppResult<List<Workspace>>.Success(workspaces);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workspaces for member {UserId}", userId);
            return AppResult<List<Workspace>>.Failure(AppError.Generic(ex.Message));
        }
    }

    public async Task<AppResult<Workspace>> UpdateWorkspaceAsync(Workspace workspace)
    {
        try
        {
            var updatedWorkspace = await _workspaceRepository.UpdateWorkspaceAsync(workspace);
            return updatedWorkspace != null 
                ? AppResult<Workspace>.Success(updatedWorkspace)
                : AppResult<Workspace>.Failure(AppError.Generic("Failed to update workspace"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating workspace {WorkspaceId}", workspace.Id);
            return AppResult<Workspace>.Failure(AppError.Generic(ex.Message));
        }
    }

    #endregion

    #region Member Management

    public async Task<AppResult<WorkspaceMemberDto>> AddMemberAsync(Guid workspaceId, AddMemberRequest request, Guid invitedBy)
    {
        try
        {
            // Validate permissions
            var canManage = await CanUserManageMembersAsync(workspaceId, invitedBy);
            if (!canManage.IsSuccess || !canManage.Value)
            {
                return AppResult<WorkspaceMemberDto>.Failure(AppError.Forbidden("Insufficient permissions to add members"));
            }

            // Check if user exists
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return AppResult<WorkspaceMemberDto>.Failure(AppError.RecordNotFound("User not found. Send an invitation instead."));
            }

            // Check if user is already a member
            var existingMember = await _workspaceMemberRepository.GetWorkspaceMemberByIdAsync(user.Id, workspaceId);
            if (existingMember != null)
            {
                return AppResult<WorkspaceMemberDto>.Failure(AppError.RecordAlreadyExists("User is already a member of this workspace"));
            }

            // Create workspace member
            var workspaceMember = new WorkspaceMember
            {
                WorkspaceId = workspaceId,
                UserId = user.Id,
                Role = request.Role,
                JoinedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdMember = await _workspaceMemberRepository.CreateWorkspaceMemberAsync(workspaceMember);
            if (createdMember == null)
            {
                return AppResult<WorkspaceMemberDto>.Failure(AppError.Generic("Failed to add member"));
            }

            var memberDto = new WorkspaceMemberDto
            {
                UserId = user.Id,
                WorkspaceId = workspaceId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = request.Role,
                JoinedAt = createdMember.JoinedAt
            };

            return AppResult<WorkspaceMemberDto>.Success(memberDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding member to workspace {WorkspaceId}", workspaceId);
            return AppResult<WorkspaceMemberDto>.Failure(AppError.Generic(ex.Message));
        }
    }

    public async Task<AppResult<WorkspaceMember>> RemoveWorkspaceMemberAsync(Guid workspaceId, Guid userId, Guid removedBy)
    {
        try
        {
            // Validate permissions
            var canManage = await CanUserManageMembersAsync(workspaceId, removedBy);
            if (!canManage.IsSuccess || !canManage.Value)
            {
                return AppResult<WorkspaceMember>.Failure(AppError.Forbidden("Insufficient permissions to remove members"));
            }

            // Cannot remove yourself if you're the only owner
            var member = await _workspaceMemberRepository.GetWorkspaceMemberByIdAsync(userId, workspaceId);
            if (member == null)
            {
                return AppResult<WorkspaceMember>.Failure(AppError.RecordNotFound("Member not found"));
            }

            if (userId == removedBy && member.Role == WorkspaceRole.Owner)
            {
                var allMembers = await _workspaceMemberRepository.GetWorkspaceMembersByWorkspaceIdAsync(workspaceId);
                var ownerCount = allMembers.Count(m => m.Role == WorkspaceRole.Owner);
                if (ownerCount <= 1)
                {
                    return AppResult<WorkspaceMember>.Failure(AppError.InvalidOperation("Cannot remove the last owner of the workspace"));
                }
            }

            // Delete member (this should use composite key but the current repo uses single Guid)
            // We'll need to update the repository interface
            var removedMember = await _workspaceMemberRepository.DeleteWorkspaceMemberAsync(userId); // This needs fixing
            return removedMember != null 
                ? AppResult<WorkspaceMember>.Success(removedMember)
                : AppResult<WorkspaceMember>.Failure(AppError.Generic("Failed to remove member"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing member from workspace {WorkspaceId}", workspaceId);
            return AppResult<WorkspaceMember>.Failure(AppError.Generic(ex.Message));
        }
    }

    public async Task<AppResult<WorkspaceMember>> UpdateWorkspaceMemberAsync(Guid workspaceId, Guid userId, WorkspaceRole role, Guid updatedBy)
    {
        try
        {
            // Validate permissions for role change
            var canChange = await CanUserChangeRoleAsync(workspaceId, updatedBy, role);
            if (!canChange.IsSuccess || !canChange.Value)
            {
                return AppResult<WorkspaceMember>.Failure(AppError.Forbidden("Insufficient permissions to change this role"));
            }

            var member = await _workspaceMemberRepository.GetWorkspaceMemberByIdAsync(userId, workspaceId);
            if (member == null)
            {
                return AppResult<WorkspaceMember>.Failure(AppError.RecordNotFound("Member not found"));
            }

            member.Role = role;
            member.UpdatedAt = DateTime.UtcNow;

            var updatedMember = await _workspaceMemberRepository.UpdateWorkspaceMemberAsync(member);
            return updatedMember != null 
                ? AppResult<WorkspaceMember>.Success(updatedMember)
                : AppResult<WorkspaceMember>.Failure(AppError.Generic("Failed to update member role"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating member role in workspace {WorkspaceId}", workspaceId);
            return AppResult<WorkspaceMember>.Failure(AppError.Generic(ex.Message));
        }
    }

    public async Task<AppResult<List<WorkspaceMemberDto>>> GetWorkspaceMembersAsync(Guid workspaceId)
    {
        try
        {
            var members = await _workspaceMemberRepository.GetWorkspaceMembersByWorkspaceIdAsync(workspaceId);
            var memberDtos = new List<WorkspaceMemberDto>();

            foreach (var member in members)
            {
                var user = await _userRepository.GetUserByIdAsync(member.UserId);
                if (user != null)
                {
                    memberDtos.Add(new WorkspaceMemberDto
                    {
                        UserId = user.Id,
                        WorkspaceId = workspaceId,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = member.Role,
                        JoinedAt = member.JoinedAt
                    });
                }
            }

            return AppResult<List<WorkspaceMemberDto>>.Success(memberDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workspace members for {WorkspaceId}", workspaceId);
            return AppResult<List<WorkspaceMemberDto>>.Failure(AppError.Generic(ex.Message));
        }
    }

    #endregion

    #region Invitation Management

    public async Task<AppResult<WorkspaceInvitationDto>> InviteUserAsync(Guid workspaceId, AddMemberRequest request, Guid invitedBy)
    {
        try
        {
            // Validate permissions
            var canManage = await CanUserManageMembersAsync(workspaceId, invitedBy);
            if (!canManage.IsSuccess || !canManage.Value)
            {
                return AppResult<WorkspaceInvitationDto>.Failure(AppError.Forbidden("Insufficient permissions to invite members"));
            }

            // Check if user is already a member
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user != null)
            {
                var existingMember = await _workspaceMemberRepository.GetWorkspaceMemberByIdAsync(user.Id, workspaceId);
                if (existingMember != null)
                {
                    return AppResult<WorkspaceInvitationDto>.Failure(AppError.RecordAlreadyExists("User is already a member of this workspace"));
                }
            }

            // Check for existing pending invitation
            var hasExisting = await _invitationRepository.HasPendingInvitationAsync(workspaceId, request.Email);
            if (hasExisting)
            {
                return AppResult<WorkspaceInvitationDto>.Failure(AppError.RecordAlreadyExists("Invitation already sent to this email"));
            }

            // Create invitation
            var invitation = new WorkspaceInvitation
            {
                Id = Guid.NewGuid(),
                WorkspaceId = workspaceId,
                Email = request.Email,
                Role = request.Role,
                InvitedBy = invitedBy,
                InvitationToken = GenerateSecureToken(),
                InvitationMessage = request.InvitationMessage,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            var createdInvitation = await _invitationRepository.CreateInvitationAsync(invitation);
            
            // Get workspace and inviter details for DTO
            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(workspaceId);
            var inviter = await _userRepository.GetUserByIdAsync(invitedBy);

            var invitationDto = new WorkspaceInvitationDto
            {
                Id = createdInvitation.Id,
                WorkspaceId = workspaceId,
                WorkspaceName = workspace?.Name ?? "Unknown Workspace",
                Email = request.Email,
                Role = request.Role,
                InvitedByName = $"{inviter?.FirstName} {inviter?.LastName}".Trim(),
                CreatedAt = createdInvitation.CreatedAt,
                ExpiresAt = createdInvitation.ExpiresAt,
                IsAccepted = false,
                InvitationMessage = request.InvitationMessage
            };

            // TODO: Send email invitation here
            _logger.LogInformation("Invitation created for {Email} to workspace {WorkspaceId}", request.Email, workspaceId);

            return AppResult<WorkspaceInvitationDto>.Success(invitationDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invitation for workspace {WorkspaceId}", workspaceId);
            return AppResult<WorkspaceInvitationDto>.Failure(AppError.Generic(ex.Message));
        }
    }

    public async Task<AppResult<WorkspaceMember>> AcceptInvitationAsync(string token, Guid userId)
    {
        try
        {
            var invitation = await _invitationRepository.GetInvitationByTokenAsync(token);
            if (invitation == null)
            {
                return AppResult<WorkspaceMember>.Failure(AppError.RecordNotFound("Invalid invitation token"));
            }

            if (invitation.IsAccepted)
            {
                return AppResult<WorkspaceMember>.Failure(AppError.RecordAlreadyExists("Invitation has already been accepted"));
            }

            if (invitation.ExpiresAt < DateTime.UtcNow)
            {
                return AppResult<WorkspaceMember>.Failure(AppError.RecordNotFound("Invitation has expired"));
            }

            // Get user and validate email matches
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null || user.Email != invitation.Email)
            {
                return AppResult<WorkspaceMember>.Failure(AppError.RecordNotFound("User email does not match invitation"));
            }

            // Check if user is already a member
            var existingMember = await _workspaceMemberRepository.GetWorkspaceMemberByIdAsync(userId, invitation.WorkspaceId);
            if (existingMember != null)
            {
                return AppResult<WorkspaceMember>.Failure(AppError.RecordAlreadyExists("User is already a member of this workspace"));
            }

            // Create workspace member
            var workspaceMember = new WorkspaceMember
            {
                WorkspaceId = invitation.WorkspaceId,
                UserId = userId,
                Role = invitation.Role,
                JoinedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdMember = await _workspaceMemberRepository.CreateWorkspaceMemberAsync(workspaceMember);
            if (createdMember == null)
            {
                return AppResult<WorkspaceMember>.Failure(AppError.Generic("Failed to accept invitation"));
            }

            // Mark invitation as accepted
            invitation.IsAccepted = true;
            invitation.AcceptedAt = DateTime.UtcNow;
            await _invitationRepository.UpdateInvitationAsync(invitation);

            return AppResult<WorkspaceMember>.Success(createdMember);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting invitation with token {Token}", token);
            return AppResult<WorkspaceMember>.Failure(AppError.Generic(ex.Message));
        }
    }

    public async Task<AppResult<bool>> CancelInvitationAsync(Guid invitationId, Guid canceledBy)
    {
        try
        {
            var invitation = await _invitationRepository.GetInvitationByIdAsync(invitationId);
            if (invitation == null)
            {
                return AppResult<bool>.Failure(AppError.RecordNotFound("Invitation not found"));
            }

            // Validate permissions
            var canManage = await CanUserManageMembersAsync(invitation.WorkspaceId, canceledBy);
            if (!canManage.IsSuccess || !canManage.Value)
            {
                return AppResult<bool>.Failure(AppError.Forbidden("Insufficient permissions to cancel invitations"));
            }

            var deleted = await _invitationRepository.DeleteInvitationAsync(invitationId);
            return AppResult<bool>.Success(deleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error canceling invitation {InvitationId}", invitationId);
            return AppResult<bool>.Failure(AppError.Generic(ex.Message));
        }
    }

    public async Task<AppResult<List<WorkspaceInvitationDto>>> GetPendingInvitationsAsync(Guid workspaceId)
    {
        try
        {
            var invitations = await _invitationRepository.GetPendingInvitationsByWorkspaceIdAsync(workspaceId);
            var invitationDtos = new List<WorkspaceInvitationDto>();

            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(workspaceId);

            foreach (var invitation in invitations)
            {
                var inviter = await _userRepository.GetUserByIdAsync(invitation.InvitedBy);
                
                invitationDtos.Add(new WorkspaceInvitationDto
                {
                    Id = invitation.Id,
                    WorkspaceId = workspaceId,
                    WorkspaceName = workspace?.Name ?? "Unknown Workspace",
                    Email = invitation.Email,
                    Role = invitation.Role,
                    InvitedByName = $"{inviter?.FirstName} {inviter?.LastName}".Trim(),
                    CreatedAt = invitation.CreatedAt,
                    ExpiresAt = invitation.ExpiresAt,
                    IsAccepted = invitation.IsAccepted,
                    InvitationMessage = invitation.InvitationMessage
                });
            }

            return AppResult<List<WorkspaceInvitationDto>>.Success(invitationDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending invitations for workspace {WorkspaceId}", workspaceId);
            return AppResult<List<WorkspaceInvitationDto>>.Failure(AppError.Generic(ex.Message));
        }
    }

    #endregion

    #region Permission Validation

    public async Task<AppResult<bool>> CanUserManageMembersAsync(Guid workspaceId, Guid userId)
    {
        try
        {
            var member = await _workspaceMemberRepository.GetWorkspaceMemberByIdAsync(userId, workspaceId);
            if (member == null)
            {
                return AppResult<bool>.Success(false);
            }

            // Owners and Admins can manage members
            var canManage = member.Role == WorkspaceRole.Owner || member.Role == WorkspaceRole.Admin;
            return AppResult<bool>.Success(canManage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking member management permissions for user {UserId} in workspace {WorkspaceId}", userId, workspaceId);
            return AppResult<bool>.Failure(AppError.Generic(ex.Message));
        }
    }

    public async Task<AppResult<bool>> CanUserChangeRoleAsync(Guid workspaceId, Guid userId, WorkspaceRole targetRole)
    {
        try
        {
            var member = await _workspaceMemberRepository.GetWorkspaceMemberByIdAsync(userId, workspaceId);
            if (member == null)
            {
                return AppResult<bool>.Success(false);
            }

            // Only owners can change roles to/from Owner
            if (targetRole == WorkspaceRole.Owner || member.Role == WorkspaceRole.Owner)
            {
                return AppResult<bool>.Success(member.Role == WorkspaceRole.Owner);
            }

            // Owners and Admins can change other roles
            var canChange = member.Role == WorkspaceRole.Owner || member.Role == WorkspaceRole.Admin;
            return AppResult<bool>.Success(canChange);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking role change permissions for user {UserId} in workspace {WorkspaceId}", userId, workspaceId);
            return AppResult<bool>.Failure(AppError.Generic(ex.Message));
        }
    }

    #endregion

    #region Private Helper Methods

    private static string GenerateSecureToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var tokenBytes = new byte[32];
        rng.GetBytes(tokenBytes);
        return Convert.ToBase64String(tokenBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    #endregion
} 