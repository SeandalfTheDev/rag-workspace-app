namespace CleverDocs.Core.Entities;

public class Workspace
{
  public Guid Id { get; set;}
  public string Name { get; set;} = string.Empty;
  public string Description { get; set;} = string.Empty;
  public DateTime CreatedAt { get; set;} = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set;} = DateTime.UtcNow;

  public virtual ICollection<WorkspaceMember> Members { get; set; } = new List<WorkspaceMember>();
}