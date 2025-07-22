---
trigger: manual
---

# Clevor Docs - Collaborative Document Intelligence Platform

## Overview

Clevor Docs is a modern, collaborative document intelligence platform that enables teams to upload, share, and query documents using AI-powered semantic search and chat capabilities. Built on a workspace-based architecture, the application allows multiple users to collaborate on shared document collections with role-based access control.

## Key Features

- **🏢 Multi-tenant Workspaces**: Organized collaboration spaces with role-based permissions
- **📄 Intelligent Document Processing**: Support for PDF, DOCX, TXT, and Markdown files
- **🤖 AI-Powered Chat**: Semantic search and question-answering using Retrieval Augmented Generation (RAG)
- **📤 Robust File Upload**: Chunked uploads for large files with real-time progress tracking
- **⚡ Real-time Collaboration**: Live chat, typing indicators, and document processing updates
- **🔐 Role-based Security**: Owner, Admin, and Member roles with granular permissions
- **🔍 Vector Search**: Semantic document search using PostgreSQL with pgvector extension
- **🎨 Modern UI**: Responsive Blazor interface with Tailwind CSS

## Tech Stack

### Backend
- **ASP.NET Core 9.0** - Web API framework
- **Entity Framework Core** - ORM for database operations
- **PostgreSQL** - Primary database with pgvector extension for vector storage
- **SignalR** - Real-time communication for chat and notifications
- **Semantic Kernel** - Microsoft's AI orchestration framework
- **Ollama** - Local LLM inference server
- **JWT Authentication** - Secure token-based authentication

### Frontend
- **Blazor Server** - Interactive web UI framework
- **Tailwind CSS** - Utility-first CSS framework
- **JavaScript Interop** - File upload and UI enhancements

### Infrastructure
- **Docker** - Containerization for easy deployment
- **Background Services** - Asynchronous document processing
- **File Storage** - Configurable local or cloud storage

## Project Architecture

### High-Level Architecture

```mermaid
graph TB
    Client[Blazor Client] --> API[ASP.NET Core API]
    Client --> SignalR[SignalR Hubs]
    API --> DB[(PostgreSQL + pgvector)]
    API --> Ollama[Ollama LLM]
    API --> Storage[File Storage]
    API --> Queue[Background Queue]
    Queue --> Processor[Document Processor]
    Processor --> Embeddings[Vector Embeddings]
    Embeddings --> DB
```

### Project Structure

```
CleverDocs/
├── src/
│   ├── CleverDocs.API/                     # Web API Layer (Minimal APIs)
│   │   ├── Modules/                        # Feature-based API modules
│   │   │   ├── Auth/                       # Authentication module
│   │   │   │   ├── AuthModule.cs           # Auth endpoints registration
│   │   │   │   ├── AuthEndpoints.cs        # Login, register, refresh endpoints
│   │   │   │   ├── AuthServices.cs         # Auth service registration
│   │   │   │   └── AuthModels.cs           # Auth request/response models
│   │   │   ├── Workspaces/                 # Workspace management module
│   │   │   │   ├── WorkspaceModule.cs      # Workspace endpoints registration
│   │   │   │   ├── WorkspaceEndpoints.cs   # CRUD and member management
│   │   │   │   ├── WorkspaceServices.cs    # Service registration
│   │   │   │   └── WorkspaceModels.cs      # Workspace DTOs
│   │   │   ├── Documents/                  # Document management module
│   │   │   │   ├── DocumentModule.cs       # Document endpoints registration
│   │   │   │   ├── DocumentEndpoints.cs    # Upload, processing, retrieval
│   │   │   │   ├── DocumentServices.cs     # Document service registration
│   │   │   │   └── DocumentModels.cs       # Document DTOs
│   │   │   ├── Chat/                       # Chat functionality module
│   │   │   │   ├── ChatModule.cs           # Chat endpoints registration
│   │   │   │   ├── ChatEndpoints.cs        # Session and message endpoints
│   │   │   │   ├── ChatServices.cs         # Chat service registration
│   │   │   │   └── ChatModels.cs           # Chat DTOs
│   │   │   └── Common/                     # Shared module components
│   │   │       ├── IModule.cs              # Module interface
│   │   │       ├── ModuleExtensions.cs     # Module registration helpers
│   │   │       └── EndpointFilters.cs      # Common endpoint filters
│   │   ├── Hubs/                           # SignalR hubs
│   │   │   ├── ChatHub.cs                  # Real-time chat
│   │   │   └── DocumentHub.cs              # Document processing updates
│   │   ├── Services/                       # Cross-cutting services
│   │   │   ├── SignalRNotificationService.cs # Real-time notifications
│   │   │   └── BackgroundTaskQueue.cs      # Background job queue
│   │   ├── Middleware/                     # Custom middleware
│   │   │   ├── ExceptionHandlingMiddleware.cs
│   │   │   └── AuthenticationMiddleware.cs
│   │   ├── Extensions/                     # Extension methods
│   │   │   ├── ServiceCollectionExtensions.cs # DI registration
│   │   │   └── WebApplicationExtensions.cs # Pipeline configuration
│   │   └── Program.cs                      # Application entry point
│   │
│   ├── CleverDocs.Client/                  # Blazor Frontend
│   │   ├── Components/                     # Reusable UI components
│   │   │   ├── Chat/                       # Chat-related components
│   │   │   │   ├── ChatInterface.razor     # Main chat UI
│   │   │   │   ├── MessageBubble.razor     # Individual messages
│   │   │   │   └── TypingIndicator.razor   # Typing status
│   │   │   ├── Documents/                  # Document-related components
│   │   │   │   ├── DocumentUpload.razor    # File upload interface
│   │   │   │   ├── DocumentList.razor      # Document browser
│   │   │   │   └── UploadProgress.razor    # Upload progress tracking
│   │   │   ├── Workspaces/                 # Workspace components
│   │   │   │   ├── WorkspaceSelector.razor
│   │   │   │   ├── MemberManagement.razor
│   │   │   │   └── WorkspaceSettings.razor
│   │   │   └── Shared/                     # Common components
│   │   │       ├── Layout/                 # Layout components
│   │   │       └── Navigation/             # Navigation components
│   │   ├── Pages/                          # Blazor pages
│   │   │   ├── Index.razor                 # Landing page
│   │   │   ├── Login.razor                 # Authentication
│   │   │   ├── Workspace.razor             # Main workspace view
│   │   │   └── Chat.razor                  # Chat interface
│   │   ├── Services/                       # Client-side services
│   │   │   ├── ApiService.cs               # HTTP API client
│   │   │   ├── SignalRService.cs           # SignalR client
│   │   │   ├── AuthenticationService.cs    # Client auth logic
│   │   │   └── StateService.cs             # Application state
│   │   ├── Models/                         # Client-side models
│   │   └── Program.cs                      # Client entry point
│   │
│   ├── CleverDocs.Core/                    # Domain Layer
│   │   ├── Entities/                       # Domain entities
│   │   │   ├── User.cs                     # User entity
│   │   │   ├── Workspace.cs                # Workspace entity
│   │   │   ├── WorkspaceMember.cs          # Membership entity
│   │   │   ├── Document.cs                 # Document entity
│   │   │   ├── DocumentChunk.cs            # Text chunk entity
│   │   │   ├── ChatSession.cs              # Chat session entity
│   │   │   └── ChatMessage.cs              # Chat message entity
│   │   ├── Interfaces/                     # Service contracts
│   │   │   ├── IDocumentService.cs
│   │   │   ├── IChatService.cs
│   │   │   ├── IWorkspaceService.cs
│   │   │   ├── IVectorSearchService.cs
│   │   │   └── IFileStorageService.cs
│   │   ├── DTOs/                           # Data transfer objects
│   │   │   ├── WorkspaceDto.cs
│   │   │   ├── DocumentDto.cs
│   │   │   ├── ChatMessageDto.cs
│   │   │   └── UserDto.cs
│   │   └── Enums/                          # Domain enumerations
│   │       ├── WorkspaceRole.cs
│   │       ├── DocumentStatus.cs
│   │       └── MessageType.cs
│   │
│   ├── CleverDocs.Infrastructure/        # Data Access Layer
│   │   ├── Data/                           # Database context and configurations
│   │   │   ├── ApplicationDbContext.cs     # EF Core context
│   │   │   ├── Configurations/             # Entity configurations
│   │   │   └── Migrations/                 # Database migrations
│   │   ├── Repositories/                   # Data access repositories
│   │   │   ├── UserRepository.cs
│   │   │   ├── WorkspaceRepository.cs
│   │   │   ├── DocumentRepository.cs
│   │   │   └── ChatRepository.cs
│   │   └── Services/                       # Infrastructure services
│   │       ├── FileStorageService.cs       # File system operations
│   │       ├── VectorSearchService.cs      # pgvector operations
│   │       └── BackgroundTaskQueue.cs      # Background processing
│   │
│   └── CleverDocs.DocumentProcessor/     # Document Processing
│       ├── Services/                       # Processing services
│       │   ├── TextExtractionService.cs    # Extract text from files
│       │   ├── ChunkingService.cs          # Split text into chunks
│       │   └── EmbeddingService.cs         # Generate vector embeddings
│       ├── Processors/                     # File type processors
│       │   ├── PdfProcessor.cs             # PDF text extraction
│       │   ├── DocxProcessor.cs            # Word document processing
│       │   └── TextProcessor.cs            # Plain text processing
│       └── Queue/                          # Background job queue
│           └── DocumentProcessingJob.cs    # Async document processing
│
├── tests/                                  # Test Projects
│   ├── CleverDocs.Tests.Unit/           # Unit tests
│   ├── CleverDocs.Tests.Integration/    # Integration tests
│   └── CleverDocs.Tests.E2E/            # End-to-end tests
│
├── docker-compose.yml                     # Docker orchestration
├── docker-compose.override.yml           # Development overrides
├── Dockerfile                            # Container definition
└── README.md                             # This file
```

## Database Schema

### Core Tables

```sql
-- Users and Authentication
users (id, email, password_hash, first_name, last_name, created_at, updated_at)

-- Workspace Management
workspaces (id, name, description, created_by, created_at, updated_at)
workspace_members (id, workspace_id, user_id, role, joined_at)

-- Document Storage
documents (id, workspace_id, filename, original_filename, file_size, 
          content_type, file_path, upload_status, uploaded_by, uploaded_at, processed_at)
document_chunks (id, document_id, chunk_index, content, embedding[vector], metadata, created_at)

-- Chat System
chat_sessions (id, workspace_id, user_id, title, created_at, updated_at)
chat_messages (id, session_id, message_type, content, metadata, created_at)
```

### Key Relationships

- **Users** belong to multiple **Workspaces** through **WorkspaceMembers**
- **Workspaces** contain multiple **Documents** and **ChatSessions**
- **Documents** are split into **DocumentChunks** for vector search
- **ChatSessions** contain multiple **ChatMessages**

## Security Model

### Authentication & Authorization

- **JWT Token Authentication**: Secure stateless authentication
- **Role-based Access Control**: Three workspace roles with distinct permissions
  - **Owner**: Full workspace control, can delete workspace
  - **Admin**: Can manage members and upload documents
  - **Member**: Can view documents and participate in chat

### Permission Matrix

| Action | Owner | Admin | Member |
|--------|-------|-------|--------|
| View Documents | ✅ | ✅ | ✅ |
| Upload Documents | ✅ | ✅ | ❌ |
| Delete Documents | ✅ | ✅ | ❌ |
| Chat Access | ✅ | ✅ | ✅ |
| Add Members | ✅ | ✅ | ❌ |
| Remove Members | ✅ | ✅ | ❌ |
| Change Roles | ✅ | ❌ | ❌ |
| Delete Workspace | ✅