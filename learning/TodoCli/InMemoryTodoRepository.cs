namespace TodoCli;

public class InMemoryTodoRepository : ITodoRepository
{
    private readonly List<TodoItem> _items = new();
    private int _nextId = 1;

    public IReadOnlyList<TodoItem> GetAll() => _items.ToList();

    public TodoItem? GetById(int id) => _items.Find(i=> i.Id ==id);

    public TodoItem Add(TodoItem item)
    {
        item.Id = _nextId++;
        _items.Add(item);
        return item;
    }

    public bool Update(TodoItem item)
    {
        var index = _items.FindIndex(i => i.Id == item.Id);
        if (index < 0) return false;

        _items[index] = item;
        return true;
    }

    public bool Delete(int id) => _items.RemoveAll(i => i.Id == id) > 0;
}