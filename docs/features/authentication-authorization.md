# User Authentication & Authorization

## Overview
The Authentication & Authorization system provides secure access control for the Clevor Docs platform. It handles user identity verification, session management, and fine-grained permission controls across workspaces and resources.

## Key Features
- Email/password authentication
- JWT-based session management
- Role-based access control (RBAC)
- Workspace-level permissions
- Session management and security
- Password reset flow
- Email verification

## Core Components

### 1. Entities

#### User
- **Purpose**: Represents a system user
- **Key Properties**:
  - `Id`: Unique identifier
  - `Email`: Unique login identifier
  - `PasswordHash`: Securely hashed password
  - `PasswordSalt`: Additional security for hashing
  - `IsEmailVerified`: Email confirmation status
  - `LastLoginAt`: Last successful login timestamp
  - `FailedLoginAttempts`: Security tracking

#### UserSession
- **Purpose**: Tracks active user sessions
- **Key Properties**:
  - `Id`: Session identifier
  - `UserId`: Associated user
  - `RefreshToken`: Token for session renewal
  - `ExpiresAt`: Session expiration
  - `Revoked`: Whether session was terminated
  - `UserAgent`: Client information
  - `IpAddress`: Client IP address

### 2. Services

#### AuthService
- **Responsibilities**:
  - User registration and profile management
  - Credential validation
  - Session creation and management
  - Token generation and validation

#### JwtService
- **Responsibilities**:
  - JWT token generation
  - Token validation and verification
  - Token refresh mechanism
  - Claims extraction and validation

#### PermissionService
- **Responsibilities**:
  - Role and permission validation
  - Policy enforcement
  - Workspace access control
  - Resource-level permissions

### 3. API Endpoints

```http
# Authentication
POST   /api/auth/register           # Register new user
POST   /api/auth/login              # User login
POST   /api/auth/refresh            # Refresh access token
POST   /api/auth/logout             # Invalidate session
POST   /api/auth/forgot-password    # Initiate password reset
POST   /api/auth/reset-password     # Complete password reset
POST   /api/auth/verify-email       # Verify email address

# User Management
GET    /api/users/me                # Get current user profile
PUT    /api/users/me                # Update profile
GET    /api/users/me/sessions       # List active sessions
DELETE /api/users/me/sessions/{id}  # Revoke session
```

## Implementation Walkthrough

### 1. User Registration
1. User submits registration form with email and password
2. System validates input and checks for existing users
3. Password is hashed with unique salt
4. User record is created with 'unverified' status
5. Verification email is sent
6. User clicks verification link to activate account

### 2. Authentication Flow
1. User submits credentials (email/password)
2. System validates credentials against stored hash
3. JWT access token and refresh token are generated
4. Session is recorded in the database
5. Tokens are returned to the client
6. Client stores tokens securely (HTTP-only cookie for refresh token)

### 3. Authorization Flow
1. Client includes access token in Authorization header
2. JWT middleware validates token and extracts claims
3. Permission service verifies user has required permissions
4. If authorized, request proceeds to handler
5. If unauthorized, 403 Forbidden is returned

## Security Considerations

### Authentication
- Strong password requirements
- Account lockout after failed attempts
- Secure password reset flow
- Email verification
- Session timeouts

### Authorization
- Principle of least privilege
- Role-based access control
- Resource-level permissions
- Audit logging of permission changes

### Session Security
- Secure, HTTP-only cookies
- CSRF protection
- Token rotation
- Session invalidation on password change

## Frontend Integration

### AuthContext.razor
- Manages authentication state
- Handles token refresh
- Provides authentication methods
- Tracks current user

### ProtectedRoute.razor
- Wrapper for protected routes
- Handles redirects for unauthenticated users
- Manages route-based permissions

### LoginForm.razor
- Email/password form
- Validation and error handling
- Password reset flow
- Social login options (if implemented)

## Error Handling
- Clear error messages for auth failures
- Rate limiting for auth endpoints
- Account lockout notifications
- Session expiration handling

## Performance Considerations
- Efficient token validation
- Caching of user permissions
- Optimized session validation
- Background cleanup of expired sessions

## Integration Points
- Email service for notifications
- Audit logging system
- User activity tracking
- Workspace membership system
