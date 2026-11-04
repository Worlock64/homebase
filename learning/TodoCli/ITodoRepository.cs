namespace TodoCli;

public interface ITodoRepository
{
    IReadOnlyList<TodoItem> GetAll();
    TodoItem? GetById (int id);
    TodoItem Add(TodoItem item);
    bool Update(TodoItem item);
    bool Delete(int id);
}