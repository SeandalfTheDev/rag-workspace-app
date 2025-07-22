# Role-Based Access Control (RBAC) Design

## Overview
This document outlines the design for a flexible and scalable Role-Based Access Control (RBAC) system for Clevor Docs.

## Core Entities

### 1. User
- `Id` (Guid)
- `Email` (string)
- `PasswordHash` (string)
- `FirstName` (string)
- `LastName` (string)
- `IsActive` (bool)
- `LastLoginAt` (DateTime?)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime)
- `UserRoles` (Collection of `UserRole`)

### 2. Role
- `Id` (Guid)
- `Name` (string) - e.g., "WorkspaceAdmin", "DocumentEditor"
- `Description` (string)
- `IsSystemRole` (bool) - Prevents modification of system roles
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime)
- `RolePermissions` (Collection of `RolePermission`)
- `UserRoles` (Collection of `UserRole`)

### 3. Permission
- `Id` (Guid)
- `Name` (string) - e.g., "Document.Create", "Workspace.Delete"
- `Description` (string)
- `Category` (string) - For grouping related permissions
- `CreatedAt` (DateTime)
- `RolePermissions` (Collection of `RolePermission`)

### 4. RolePermission (Junction Table)
- `RoleId` (Guid, FK to Role)
- `PermissionId` (Guid, FK to Permission)
- `CreatedAt` (DateTime)

### 5. UserRole (Junction Table)
- `UserId` (Guid, FK to User)
- `RoleId` (Guid, FK to Role)
- `ScopeId` (Guid?) - For scoping roles to specific resources (e.g., workspaceId)
- `ScopeType` (string) - e.g., "Workspace", "Document"
- `CreatedAt` (DateTime)
- `ExpiresAt` (DateTime?) - For temporary role assignments

## Permission Design

### Permission Naming Convention
`{Resource}.{Action}`

### Example Permissions
```
// Workspace Permissions
Workspace.Create
Workspace.Read
Workspace.Update
Workspace.Delete
Workspace.InviteMember
Workspace.ManageMembers

// Document Permissions
Document.Upload
Document.Read
Document.Update
Document.Delete
Document.Share
Document.Process

// Chat Permissions
Chat.Create
Chat.Read
Chat.Delete
Chat.Manage
```

## Role Design

### System Roles
1. **SuperAdmin**
   - Full access to all features and settings
   - Can manage all workspaces and users

2. **WorkspaceOwner**
   - Full access to a specific workspace
   - Can manage workspace settings and members

3. **WorkspaceAdmin**
   - Can manage most workspace settings
   - Can invite/remove members
   - Cannot delete the workspace

4. **Member**
   - Basic access to workspace features
   - Can create and edit documents
   - Cannot manage workspace settings

### Custom Roles
Admins can create custom roles by combining any set of permissions.

## Scoped Roles
Roles can be scoped to specific resources using the `ScopeId` in the `UserRole` table:
- Workspace-level roles
- Document-level roles
- Folder-level roles

## Implementation Details

### Database Schema
```sql
CREATE TABLE Users (
    Id UUID PRIMARY KEY,
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    FirstName VARCHAR(100),
    LastName VARCHAR(100),
    IsActive BOOLEAN DEFAULT true,
    LastLoginAt TIMESTAMP,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Roles (
    Id UUID PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE,
    Description TEXT,
    IsSystemRole BOOLEAN DEFAULT false,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Permissions (
    Id UUID PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE,
    Description TEXT,
    Category VARCHAR(100),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE RolePermissions (
    RoleId UUID REFERENCES Roles(Id) ON DELETE CASCADE,
    PermissionId UUID REFERENCES Permissions(Id) ON DELETE CASCADE,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (RoleId, PermissionId)
);

CREATE TABLE UserRoles (
    UserId UUID REFERENCES Users(Id) ON DELETE CASCADE,
    RoleId UUID REFERENCES Roles(Id) ON DELETE CASCADE,
    ScopeId UUID NULL, -- For scoped roles (e.g., workspaceId)
    ScopeType VARCHAR(50) NULL, -- e.g., 'Workspace', 'Document'
    ExpiresAt TIMESTAMP NULL, -- For temporary roles
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (UserId, RoleId, ScopeId, ScopeType)
);
```

### Permission Evaluation
```csharp
public class PermissionService : IPermissionService
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;

    public async Task<bool> HasPermissionAsync(
        Guid userId, 
        string permissionName, 
        Guid? scopeId = null, 
        string scopeType = null)
    {
        // Get all roles for the user (both global and scoped)
        var userRoles = await _userRoleRepository.GetUserRolesAsync(userId, scopeId, scopeType);
        
        // Get all permissions for these roles
        var roleIds = userRoles.Select(ur => ur.RoleId).Distinct().ToList();
        var permissions = await _rolePermissionRepository.GetPermissionsForRolesAsync(roleIds);
        
        // Check if any permission matches
        return permissions.Any(p => 
            p.Name.Equals(permissionName, StringComparison.OrdinalIgnoreCase));
    }
}
```

### Authorization Attribute
```csharp
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizePermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _permission;
    private readonly string _scopeType;

    public AuthorizePermissionAttribute(string permission, string scopeType = null)
    {
        _permission = permission;
        _scopeType = scopeType;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get scopeId from route/query/body based on scopeType
        Guid? scopeId = GetScopeIdFromRequest(context, _scopeType);
        
        var permissionService = context.HttpContext.RequestServices
            .GetRequiredService<IPermissionService>();
            
        var hasPermission = permissionService.HasPermissionAsync(
            Guid.Parse(userId), 
            _permission, 
            scopeId, 
            _scopeType).Result;

        if (!hasPermission)
        {
            context.Result = new ForbidResult();
        }
    }
}
```

## Usage Examples

### Controller Authorization
```csharp
[ApiController]
[Route("api/workspaces")]
public class WorkspaceController : ControllerBase
{
    [AuthorizePermission("Workspace.Read")]
    [HttpGet("{workspaceId}")]
    public async Task<IActionResult> GetWorkspace(Guid workspaceId)
    {
        // User has Workspace.Read permission
    }
    
    [AuthorizePermission("Document.Upload", "Workspace")]
    [HttpPost("{workspaceId}/documents/upload")]
    public async Task<IActionResult> UploadDocument(Guid workspaceId, IFormFile file)
    {
        // User has Document.Upload permission for this workspace
    }
}
```

### Service Layer Authorization
```csharp
public class DocumentService : IDocumentService
{
    private readonly IPermissionService _permissionService;
    private readonly ICurrentUserService _currentUserService;
    
    public async Task<Document> GetDocumentAsync(Guid documentId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        
        // Check if user has permission to read this document
        var hasPermission = await _permissionService.HasPermissionAsync(
            _currentUserService.UserId,
            "Document.Read",
            document.WorkspaceId,
            "Workspace");
            
        if (!hasPermission)
        {
            throw new UnauthorizedAccessException("You don't have permission to view this document");
        }
        
        return document;
    }
}
```

## Next Steps
1. Implement the database schema and entities
2. Create migration scripts
3. Implement the repositories and services
4. Add seed data for system roles and permissions
5. Implement the authorization middleware
6. Add unit and integration tests
