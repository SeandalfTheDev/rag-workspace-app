namespace CleverDocs.Core.Entities;

public class User
{
  public Guid Id { get; set;}
  public string FirstName { get; set;} = string.Empty;
  public string LastName { get; set;} = string.Empty;
  public string Email { get; set;} = string.Empty;
  public string PasswordHash { get; set;} = string.Empty;
  public DateTime CreatedAt { get; set;} = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set;} = DateTime.UtcNow;
}