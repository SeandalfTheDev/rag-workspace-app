# Clevor Docs - Entity Models & Classes

This document outlines the core entity models and classes required for the Clevor Docs platform, a collaborative document intelligence solution.

## Core Domain Models

### 1. User Management

#### User
- `Id` (Guid) - Unique identifier
- `Email` (string) - User's email address (unique)
- `PasswordHash` (byte[]) - Hashed password
- `PasswordSalt` (byte[]) - Password salt
- `FirstName` (string) - User's first name
- `LastName` (string) - User's last name
- `AvatarUrl` (string) - URL to user's avatar
- `IsEmailVerified` (bool) - Email verification status
- `CreatedAt` (DateTime) - Account creation timestamp
- `LastLoginAt` (DateTime?) - Last login timestamp

#### UserSession
- `Id` (Guid) - Session identifier
- `UserId` (Guid) - Reference to User
- `RefreshToken` (string) - Refresh token value
- `ExpiresAt` (DateTime) - Expiration timestamp
- `CreatedAt` (DateTime) - Creation timestamp
- `Revoked` (bool) - Whether session was revoked
- `IpAddress` (string) - IP address of the client
- `UserAgent` (string) - Client user agent

### 2. Workspace Management

#### Workspace
- `Id` (Guid) - Unique identifier
- `Name` (string) - Workspace name
- `Description` (string) - Workspace description
- `Slug` (string) - URL-friendly identifier
- `CreatedAt` (DateTime) - Creation timestamp
- `UpdatedAt` (DateTime) - Last update timestamp
- `CreatedById` (Guid) - Reference to creator User

#### WorkspaceMember
- `Id` (Guid) - Unique identifier
- `WorkspaceId` (Guid) - Reference to Workspace
- `UserId` (Guid) - Reference to User
- `Role` (enum: Owner/Admin/Member) - Member role
- `JoinedAt` (DateTime) - When user joined the workspace
- `InvitedById` (Guid?) - Who invited this member
- `Status` (enum: Active/Pending/Invited) - Member status

#### WorkspaceInvitation
- `Id` (Guid) - Unique identifier
- `WorkspaceId` (Guid) - Reference to Workspace
- `Email` (string) - Email address of invitee
- `InvitedById` (Guid) - Reference to User who sent the invitation
- `Role` (enum: Admin/Member) - Role being offered
- `Token` (string) - Unique token for invitation URL
- `ExpiresAt` (DateTime) - When the invitation expires
- `Status` (enum: Pending/Accepted/Expired/Revoked) - Current status
- `CreatedAt` (DateTime) - When the invitation was created
- `UpdatedAt` (DateTime) - Last update timestamp
- `Metadata` (JSON) - Additional invitation metadata

### 3. Document Management

#### Document
- `Id` (Guid) - Unique identifier
- `WorkspaceId` (Guid) - Reference to Workspace
- `UploadedById` (Guid) - Reference to User who uploaded
- `FileName` (string) - Original filename
- `FileType` (string) - File extension/type
- `FileSize` (long) - File size in bytes
- `StoragePath` (string) - Path in storage
- `Status` (enum: Uploading/Processing/Ready/Error) - Processing status
- `Pages` (int) - Number of pages (if applicable)
- `Metadata` (JSON) - Additional metadata
- `CreatedAt` (DateTime) - Upload timestamp
- `ProcessedAt` (DateTime?) - When processing completed
- `Error` (string) - Error message if processing failed

#### DocumentChunk
- `Id` (Guid) - Unique identifier
- `DocumentId` (Guid) - Reference to Document
- `ChunkIndex` (int) - Order of chunk in document
- `Content` (string) - Text content of chunk
- `PageNumber` (int?) - Source page number
- `VectorEmbedding` (float[]) - Vector representation
- `Metadata` (JSON) - Additional chunk metadata

### 4. Chat & Search

#### ChatSession
- `Id` (Guid) - Unique identifier
- `WorkspaceId` (Guid) - Reference to Workspace
- `CreatedById` (Guid) - Reference to User
- `Title` (string) - Generated from first message
- `CreatedAt` (DateTime) - Creation timestamp
- `UpdatedAt` (DateTime) - Last activity timestamp
- `IsPinned` (bool) - Whether session is pinned
- `DocumentContext` (JSON) - Relevant document IDs

#### ChatMessage
- `Id` (Guid) - Unique identifier
- `SessionId` (Guid) - Reference to ChatSession
- `SenderId` (Guid?) - Reference to User (null for AI)
- `Content` (string) - Message text
- `IsFromAI` (bool) - Whether message is from AI
- `SentAt` (DateTime) - Timestamp
- `References` (JSON) - Document chunks referenced
- `Metadata` (JSON) - Additional metadata

## Service Layer Classes

### 1. Authentication Services
- `AuthService` - Handles user registration, login, and token management
- `JwtService` - JWT token generation and validation
- `PasswordService` - Password hashing and verification

### 2. Document Services
- `DocumentUploadService` - Manages file uploads and storage
- `DocumentProcessingService` - Handles document text extraction and chunking
- `VectorEmbeddingService` - Generates and manages vector embeddings
- `DocumentSearchService` - Handles semantic search across documents

### 3. AI Services
- `ChatService` - Manages chat sessions and message history
- `RagService` - Implements Retrieval Augmented Generation
- `EmbeddingService` - Interface with embedding models
- `CompletionService` - Interface with LLM completion models

### 4. Workspace Services
- `WorkspaceService` - Manages workspace CRUD operations
- `PermissionService` - Handles role-based access control
- `InvitationService` - Manages workspace invitations

## Data Transfer Objects (DTOs)

### Request DTOs
- `LoginRequest` - User login credentials
- `RegisterRequest` - New user registration
- `CreateWorkspaceRequest` - New workspace details
- `UploadDocumentRequest` - Document upload metadata
- `ChatMessageRequest` - New chat message
- `SearchRequest` - Document search parameters

### Response DTOs
- `AuthResponse` - Authentication tokens and user info
- `WorkspaceDto` - Workspace details
- `DocumentDto` - Document metadata
- `ChatSessionDto` - Chat session info
- `SearchResultDto` - Search results

## Event Models

### Domain Events
- `UserRegisteredEvent` - Triggered on new user registration
- `DocumentUploadedEvent` - Triggered when document is uploaded
- `DocumentProcessedEvent` - Triggered when processing completes
- `ChatMessageSentEvent` - Triggered on new chat message

## Database Context

### `AppDbContext`
- `Users` - DbSet<User>
- `Workspaces` - DbSet<Workspace>
- `WorkspaceMembers` - DbSet<WorkspaceMember>
- `WorkspaceInvitations` - DbSet<WorkspaceInvitation>
- `Documents` - DbSet<Document>
- `DocumentChunks` - DbSet<DocumentChunk>
- `ChatSessions` - DbSet<ChatSession>
- `ChatMessages` - DbSet<ChatMessage>
- `UserSessions` - DbSet<UserSession>

## Integration Models

### Ollama Integration
- `OllamaCompletionRequest` - Request format for Ollama API
- `OllamaCompletionResponse` - Response from Ollama API
- `OllamaEmbeddingRequest` - Request for generating embeddings
- `OllamaEmbeddingResponse` - Response containing embeddings

### Storage Integration
- `FileUploadResult` - Result of file upload operation
- `StorageOptions` - Configuration for storage providers
- `FileMetadata` - Metadata about stored files

## Security Models

### Claims
- `UserClaims` - Standard JWT claims
- `CustomClaims` - Application-specific claims

### Policies
- `WorkspaceAccessPolicy` - Controls workspace access
- `DocumentAccessPolicy` - Controls document access
- `AdminOnlyPolicy` - Restricts to admin users

## View Models (for Blazor Components)

### Pages
- `LoginViewModel` - Login page
- `WorkspaceListViewModel` - Workspace dashboard
- `DocumentListViewModel` - Document management
- `ChatViewModel` - Chat interface
- `SearchViewModel` - Search interface

### Components
- `DocumentUploaderViewModel` - File upload component
- `ChatMessageViewModel` - Chat message display
- `DocumentViewerViewModel` - Document preview
- `UserMenuViewModel` - User dropdown menu

---

This model architecture provides a solid foundation for implementing the Clevor Docs platform, with clear separation of concerns and scalability in mind. The design supports the key features of document management, AI-powered search, and real-time collaboration while maintaining security and performance.
