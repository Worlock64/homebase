namespace TodoCli;
// Represents the priority level of a todo item.
public enum Priority
{
    Low,
    Medium,
    High
}

// Represents a single todo item with properties for ID, title, priority, completion status, and creation timestamp.
public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Priority Priority { get; set; } =Priority.Medium;
    public bool IsDone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}