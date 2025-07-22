# Chat & Conversation

## Overview
The Chat & Conversation system enables real-time, context-aware communication between users and AI within workspaces. It leverages document context for intelligent responses and supports rich media interactions.

## Key Features
- Real-time messaging with typing indicators
- Document context awareness
- Threaded conversations
- Message history and persistence
- Rich media support (images, code blocks, file attachments)
- Message reactions and interactions

## Core Components

### 1. Entities

#### ChatSession
- **Purpose**: Represents a conversation thread
- **Key Properties**:
  - `Id`: Unique identifier
  - `WorkspaceId`: Associated workspace
  - `Title`: Generated from first message
  - `CreatedAt/UpdatedAt`: Timestamps
  - `IsPinned`: Favorite status
  - `DocumentContext`: Related document IDs

#### ChatMessage
- **Purpose**: Individual message in a conversation
- **Key Properties**:
  - `Id`: Unique identifier
  - `SessionId`: Parent conversation
  - `SenderId`: User who sent the message
  - `Content`: Message text
  - `IsFromAI`: Whether from AI or user
  - `SentAt`: Timestamp
  - `References`: Related document chunks
  - `Metadata`: Additional context

### 2. Services

#### ChatService
- **Responsibilities**:
  - Manage chat sessions and messages
  - Handle message persistence
  - Enforce message retention policies
  - Provide conversation history

#### RAGService (Retrieval-Augmented Generation)
- **Responsibilities**:
  - Generate context-aware responses
  - Retrieve relevant document chunks
  - Format responses with references

#### SignalRHub
- **Responsibilities**:
  - Real-time message broadcasting
  - Typing indicators
  - Online status updates
  - Message read receipts

### 3. API Endpoints

```http
# Chat Sessions
GET    /api/workspaces/{workspaceId}/chats          # List conversations
POST   /api/workspaces/{workspaceId}/chats          # Start new chat
GET    /api/workspaces/{workspaceId}/chats/{id}     # Get chat details
DELETE /api/workspaces/{workspaceId}/chats/{id}     # Delete chat

# Messages
GET    /api/workspaces/{workspaceId}/chats/{id}/messages  # Get messages
POST   /api/workspaces/{workspaceId}/chats/{id}/messages  # Send message

# Real-time
POST   /api/workspaces/{workspaceId}/chats/{id}/typing    # Typing indicator
POST   /api/workspaces/{workspaceId}/chats/{id}/read      # Mark as read
```

## Implementation Walkthrough

### 1. Starting a New Chat
1. User initiates a new chat from the UI
2. System creates a new ChatSession record
3. Initial context is established (workspace, documents)
4. Empty chat interface is presented

### 2. Sending a Message
1. User composes and sends a message
2. Message is immediately displayed optimistically
3. Request is sent to the server
4. Message is persisted to the database
5. Real-time update is broadcast to all participants
6. If AI response is needed, RAG pipeline is triggered
7. AI response is generated and sent back

### 3. Context Management
1. System maintains conversation context
2. Relevant document chunks are retrieved
3. Context window is managed for LLM constraints
4. Conversation history is summarized when needed

## Frontend Components

### ChatInterface.razor
- Main chat container
- Message list with virtualization
- Message composition area
- Contextual suggestions

### MessageBubble.razor
- Individual message display
- Timestamp and sender info
- Message actions (reply, react, etc.)
- Status indicators (sent, delivered, read)

### ChatSidebar.razor
- List of conversations
- Search and filter functionality
- New chat button
- Conversation metadata (unread count, last message)

## Security Considerations
- End-to-end message encryption
- Rate limiting for API endpoints
- Input sanitization
- Access control at workspace level
- Message retention policies

## Performance Considerations
- Message pagination
- Virtualized rendering for message lists
- WebSocket connection management
- Efficient context window management
- Caching of frequent queries

## Integration Points
- Document system for context retrieval
- User system for identity and presence
- Notification system for mentions and updates
- Analytics for conversation metrics

## Error Handling
- Failed message queuing and retry
- Connection state management
- Graceful degradation when AI is unavailable
- Clear error messages for users
