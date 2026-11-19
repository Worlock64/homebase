namespace TodoCli;

public class LoggingTodoRepository : ITodoRepository
{
    private readonly ITodoRepository _innerRepository;

    public LoggingTodoRepository(ITodoRepository innerRepository)
    {
        _innerRepository = innerRepository;
    }

    public Task<IReadOnlyList<TodoItem>> GetAllAsync()
    {
        Console.WriteLine("[repo] GetAllAsync called");
        return _innerRepository.GetAllAsync();
    }

    public Task<TodoItem?> GetByIdAsync(int id)
    {
        Console.WriteLine($"[repo] GetByIdAsync called with id: {id}");
        return _innerRepository.GetByIdAsync(id);
    }

    public Task<TodoItem> AddAsync(TodoItem item)
    {
        Console.WriteLine($"[repo] AddAsync called with item: {item.Title}");
        return _innerRepository.AddAsync(item);
    }

    public Task<bool> UpdateAsync(TodoItem item)
    {
        Console.WriteLine($"[repo] UpdateAsync called with item: {item.Title}");
        return _innerRepository.UpdateAsync(item);
    }

    public Task<bool> DeleteAsync(int id)
    {
        Console.WriteLine($"[repo] DeleteAsync called with id: {id}");
        return _innerRepository.DeleteAsync(id);
    }
}