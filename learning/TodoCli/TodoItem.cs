namespace TodoCli;

public enum Priority
{
    Low,
    Medium,
    High
}

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Priority Priority { get; set; } =Priority.Medium;
    public bool IsDone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}