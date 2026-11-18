namespace TodoCli;

public class InMemoryTodoRepository : ITodoRepository
{
    private readonly List<TodoItem> _items = new();
    private int _nextId = 1;

    public Task<IReadOnlyList<TodoItem>> GetAllAsync() => Task.FromResult<IReadOnlyList<TodoItem>>(_items.ToList());

    public Task<TodoItem?> GetByIdAsync(int id) => Task.FromResult<TodoItem?>(_items.Find(i=> i.Id ==id));

    public Task<TodoItem> AddAsync(TodoItem item)
    {
        item.Id = _nextId++;
        _items.Add(item);
        return Task.FromResult(item);
    }

    public Task<bool> UpdateAsync(TodoItem item)
    {
        var index = _items.FindIndex(i => i.Id == item.Id);
        if (index < 0) return Task.FromResult(false);

        _items[index] = item;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id) => Task.FromResult(_items.RemoveAll(i => i.Id == id) > 0);
}