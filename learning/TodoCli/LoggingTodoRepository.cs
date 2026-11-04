namespace TodoCli;

public class LoggingTodoRepository : ITodoRepository
{
    private readonly ITodoRepository _innerRepository;

    public LoggingTodoRepository(ITodoRepository innerRepository)
    {
        _innerRepository = innerRepository;
    }

    public IReadOnlyList<TodoItem> GetAll()
    {
        Console.WriteLine("[repo] GetAll called");
        return _innerRepository.GetAll();
    }

    public TodoItem? GetById(int id)
    {
        Console.WriteLine($"[repo] GetById called with id: {id}");
        return _innerRepository.GetById(id);
    }

    public TodoItem Add(TodoItem item)
    {
        Console.WriteLine($"[repo] Add called with item: {item.Title}");
        return _innerRepository.Add(item);
    }

    public bool Update(TodoItem item)
    {
        Console.WriteLine($"[repo] Update called with item: {item.Title}");
        return _innerRepository.Update(item);
    }

    public bool Delete(int id)
    {
        Console.WriteLine($"[repo] Delete called with id: {id}");
        return _innerRepository.Delete(id);
    }
}