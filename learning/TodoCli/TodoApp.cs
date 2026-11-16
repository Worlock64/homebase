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
            Console.WriteLine("\t=== Todo CLI ===");
            Console.WriteLine("\t1)List\n \t2)Add\n \t3)Complete\n \t4)Delete\n \t5)Update\n \ts)Search/Filter\n \tr)Show Report\n \t6)Quit\n");
            Console.Write("> ");
            var input = Console.ReadLine()?.Trim().ToLowerInvariant() ?? string.Empty;

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
                case "s":
                    SearchItems();
                    break;
                case "r":
                    ShowReport();
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
        PrintItems(_repository.GetAll());
    }

    private void SearchItems()
    {
        Console.Write("Search text (blank for any): ");
        var text = Console.ReadLine()?.Trim() ?? string.Empty;

        Console.Write("Show (all/open/done): ");
        var status = Console.ReadLine()?.Trim().ToLowerInvariant();

        Console.Write("Sort by (priority/created date/Title):");
        var sortBy = Console.ReadLine()?.Trim().ToLowerInvariant();

        Console.Write("Any high priority items? (y/n): ");
        var anyHighPriority = Console.ReadLine()?.Trim().ToLowerInvariant();

        var query = _repository.GetAll().AsEnumerable();

        if (text.Length > 0)
        {
            query = query.Where(i => i.Title.Contains(text, StringComparison.OrdinalIgnoreCase));
        }

        query = status switch
        {
            "open" => query.Where(i => !i.IsDone),
            "done" => query.Where(i => i.IsDone),
            _ => query
        };

        if (anyHighPriority == "y")
        {
            var hasHighPriority = query.Any(i => i.Priority == Priority.High);
            if(hasHighPriority)
            {
                Console.WriteLine("There are high priority items in the list.");
            }
            else
            {
                Console.WriteLine("No high priority items found.");
            }
        }

        IReadOnlyCollection<TodoItem> results;

        switch(sortBy)
        {
            case "priority":
            default:
                results = query
                    .OrderByDescending(i => i.Priority)
                    .ThenBy(i => i.CreatedAt)
                    .ToList();
                break;
            case "created date":
                results = query
                    .OrderBy(i => i.CreatedAt)
                    .ThenByDescending(i => i.Priority)
                    .ToList();
                break;
            case "title":
                results = query
                    .OrderBy(i => i.Title)
                    .ThenByDescending(i => i.Priority)
                    .ToList();
                break;
        }

        PrintItems(results);
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

    private void ShowReport()
    {
        var items = _repository.GetAll();
        
        if(items.Count == 0)
        {
            Console.WriteLine("No data yet.");
            return;
        }

        var doneCount = items.Count(i => i.IsDone);
        Console.WriteLine($"Total: {items.Count}, Done: {doneCount}, Open: {items.Count - doneCount}");
        Console.WriteLine($"Completion: {doneCount * 100 / items.Count}%");

        Console.WriteLine();
        foreach (var group in items.GroupBy(i=> i.Priority).OrderByDescending(g => g.Key))
        {
            var open = group.Count(i => !i.IsDone);
            Console.WriteLine($"{group.Key,-8} {group.Count(),3} total, {open} open");
        }

        var oldestOpen = items
            .Where(i=>!i.IsDone)
            .OrderBy(i=>i.CreatedAt)
            .FirstOrDefault();

            Console.WriteLine();
            Console.WriteLine(oldestOpen is null
                ? "Oldest open item: None, Everything is done!"
                : $"Oldest open item: {oldestOpen.Title} (added {oldestOpen.CreatedAt:yyyy-MM-dd})");

        var newestAdded = items
            .OrderByDescending(i=>i.CreatedAt)
            .ThenByDescending(i=>i.Id)
            .Take(3)
            .ToList();

            Console.WriteLine();
            Console.WriteLine("Newest added items:");
            
            PrintItems(newestAdded);
    }

    private static bool TryReadId(out int id)
    {
        Console.Write("Id: ");
        if(int.TryParse(Console.ReadLine(), out id)) return true;

        Console.WriteLine("Please enter a valid number for the ID.");
        return false;
    }

    private static void PrintItems(IReadOnlyCollection<TodoItem> items)
    {
        if (items.Count == 0)
        {
            Console.WriteLine("Nothing to show.");
            return;
        }

        foreach (var item in items)
        {
            var mark = item.IsDone ? "x" : " ";
            Console.WriteLine($"[{mark}] {item.Id,3} {item.Title} ({item.Priority})");
        }
    }
}