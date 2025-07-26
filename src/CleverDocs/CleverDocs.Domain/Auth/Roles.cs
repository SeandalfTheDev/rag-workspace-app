namespace CleverDocs.Domain.Auth;

public static class Roles
{
    public const string AppAdmin = nameof(AppAdmin);
    public const string AppUser = nameof(AppUser);
    
    public const string WorkspaceOwner = nameof(WorkspaceOwner);
    public const string WorkspaceAdmin = nameof(WorkspaceAdmin);
    public const string WorkspaceMember = nameof(WorkspaceMember);
}