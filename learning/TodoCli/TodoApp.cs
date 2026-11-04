namespace TodoCli;

public class TodoApp
{
    private readonly ITodoRepository _repository;

    public TodoApp(ITodoRepository repository)
    {
        _repository = repository;
    }

    public void Run()
    {
        var isRunning = true;

        while(isRunning)
        {
            Console.WriteLine();
            Console.WriteLine("=== Todo CLI ===");
            Console.WriteLine(" 1)List\n 2)Add\n 3)Complete\n 4)Delete\n 5)Update\n 6)Quit\n");
            Console.Write("> ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    ListItems();
                    break;
                case "2":
                    AddItem();
                    break;
                case "3":
                    CompleteItem();
                    break;
                case "4":
                    DeleteItem();
                    break;
                case "5":
                    UpdateItem();
                    break;
                case "6":
                    isRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private void ListItems()
    {
        var items = _repository.GetAll();
        if (items.Count == 0)
        {
            Console.WriteLine("Nothing to do yet.");
            return;
        }

        Console.WriteLine("Todo Items:");
        foreach (var item in items)
        {
            var status = item.IsDone ? "x" : "";
            Console.WriteLine($"[{status}] {item.Id,3} {item.Title} ({item.Priority})");
        }
    }

    private void AddItem()
    {
        Console.Write("Title: ");
        var title = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(title))
        {
            Console.WriteLine("Title cannot be empty.");
            return;
        }

        Console.Write("Priority (Low, Medium, High): ");
        var priorityInput = Console.ReadLine()?.Trim();
        var priority = Enum.TryParse<Priority>(priorityInput, true, out var parsedPriority)
            ? parsedPriority
            : Priority.Medium;

        _repository.Add(new TodoItem { Title = title, Priority = priority });
        Console.WriteLine("Added: " + title);
    }

    private void CompleteItem()
    {
        if(!TryReadId(out var id)) return;

        var item = _repository.GetById(id);
        if (item is null)
        {
            Console.WriteLine($"No item found with ID {id}.");
            return;
        }

        item.IsDone = true;
        _repository.Update(item);
        Console.WriteLine($"Marked item {id} as complete.");
    }

    private void UpdateItem()
    {
        if(!TryReadId(out int id)) return;

        var item = _repository.GetById(id);
        if (item is null)
        {
            Console.WriteLine($"No item found with ID {id}.");
            return;
        }

        Console.WriteLine("Current Title: " + item.Title);
        Console.Write("New Title (leave empty to keep current): ");
        var newTitle = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(newTitle))
        {
            item.Title = newTitle;
        }

        Console.WriteLine("Current Priority: " + item.Priority);
        Console.Write("New Priority (Low, Medium, High, leave empty to keep current): ");
        var newPriorityInput = Console.ReadLine()?.Trim();

        if (Enum.TryParse<Priority>(newPriorityInput, true, out var newPriority))
        {
            item.Priority = newPriority;
        }

        _repository.Update(item);
        Console.WriteLine($"Updated item {id}.");
    }

    private void DeleteItem()
    {
        Console.WriteLine(TryReadId(out int id) && _repository.Delete(id)
            ? $"Deleted item {id}."
            : $"No item found with ID {id}.");
    }

    private static bool TryReadId(out int id)
    {
        Console.Write("Id: ");
        if(int.TryParse(Console.ReadLine(), out id)) return true;

        Console.WriteLine("Please enter a valid number for the ID.");
        return false;
    }
}