# Workspace Management

## Overview
Workspace Management is a core feature of Clevor Docs that enables collaborative document intelligence through organized, multi-user workspaces. Each workspace acts as an isolated environment where team members can collaborate on documents with controlled access and permissions.

## Key Features
- Create, update, and delete workspaces
- Invite and manage workspace members with different roles (Owner/Admin/Member)
- Control access to documents and features based on roles
- Track workspace activity and changes

## Core Components

### 1. Entities

#### Workspace
- **Purpose**: Represents a collaborative space for document management
- **Key Properties**:
  - `Id`: Unique identifier
  - `Name`: Workspace display name
  - `Description`: Detailed description
  - `Slug`: URL-friendly identifier
  - `CreatedAt/UpdatedAt`: Timestamps
  - `CreatedById`: Reference to the creator

#### WorkspaceMember
- **Purpose**: Manages user membership and permissions within a workspace
- **Key Properties**:
  - `WorkspaceId`: Reference to the workspace
  - `UserId`: Reference to the user
  - `Role`: Permission level (Owner/Admin/Member)
  - `Status`: Membership status (Active/Pending/Invited)
  - `InvitedById`: Reference to the inviter

#### WorkspaceInvitation
- **Purpose**: Tracks pending invitations to join a workspace
- **Key Properties**:
  - `WorkspaceId`: Reference to the target workspace
  - `Email`: Email address of the invitee
  - `Role`: Role being offered (Admin/Member)
  - `Token`: Unique token for the invitation URL
  - `ExpiresAt`: Expiration timestamp
  - `Status`: Current status (Pending/Accepted/Expired/Revoked)
  - `InvitedById`: Reference to the user who sent the invitation

### 2. Services

#### WorkspaceService
- **Responsibilities**:
  - CRUD operations for workspaces
  - Workspace metadata management
  - Workspace settings configuration

#### PermissionService
- **Responsibilities**:
  - Role-based access control
  - Permission validation for workspace actions
  - Policy enforcement

#### InvitationService
- **Responsibilities**:
  - Sending and managing workspace invitations
  - Generating secure invitation tokens
  - Handling invitation acceptance/decline
  - Managing pending and expired invitations
  - Validating invitation tokens
  - Sending email notifications for invitations

### 3. API Endpoints

```http
# Workspace Management
GET    /api/workspaces                 # List workspaces
POST   /api/workspaces                 # Create workspace
GET    /api/workspaces/{id}            # Get workspace details
PUT    /api/workspaces/{id}            # Update workspace
DELETE /api/workspaces/{id}            # Delete workspace

# Workspace Members
GET    /api/workspaces/{id}/members    # List members
POST   /api/workspaces/{id}/members    # Add member
PUT    /api/workspaces/{id}/members/{userId}  # Update member role
DELETE /api/workspaces/{id}/members/{userId}  # Remove member

# Invitations
POST   /api/workspaces/{id}/invite     # Send invitation
GET    /api/workspaces/{id}/invitations # List pending invitations
DELETE /api/workspaces/{id}/invitations/{invitationId} # Cancel invitation
```

## Implementation Walkthrough

### 1. Workspace Creation
1. User submits a request to create a new workspace with name and description
2. System validates the request and checks for duplicate names
3. A new Workspace record is created with the user as the Owner
4. A WorkspaceMember record is created with Owner role
5. The workspace is now ready for use and the creator can invite other members

### 2. Inviting Members
1. Workspace owner/admin initiates an invitation by providing an email and role
2. System validates the request and checks for existing memberships
3. A new `WorkspaceInvitation` record is created with:
   - A unique token for the invitation URL
   - An expiration time (typically 7 days)
   - The specified role (Admin/Member)
   - Status set to 'Pending'
4. An email is sent to the invitee with an acceptance link
5. When the invitee clicks the link:
   - The token is validated
   - If valid, a new account is created (if needed)
   - A `WorkspaceMember` record is created
   - The invitation status is updated to 'Accepted'
   - The user gains access to the workspace

### 3. Access Control Flow
1. User attempts to access a workspace resource
2. Authentication middleware validates the user's token
3. PermissionService checks if the user has the required role/permission
4. If authorized, the request proceeds; otherwise, access is denied

## Frontend Components

### WorkspaceList.razor
- Displays all workspaces the user has access to
- Provides options to create new workspaces
- Shows workspace activity and member count

### WorkspaceSettings.razor
- Manages workspace details and settings
- Handles workspace deletion (owners only)

### MemberManagement.razor
- Lists current members and their roles
- Provides interface for managing members and sending invitations
- Allows role changes and member removal

## Security Considerations
- Only workspace owners can delete workspaces
- Only owners/admins can invite new members or change roles
- Role changes require appropriate permissions
- Invitation tokens are single-use and expire after a set period
- All API endpoints are protected with JWT authentication and role-based authorization
- Invitation emails include security information and sender details
- Users cannot be invited multiple times with different roles

## Performance Considerations
- Workspace lists are paginated for performance
- Member lists are loaded asynchronously
- Caching is implemented for frequently accessed workspace data
