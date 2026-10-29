using TodoCli;

var items = new List<TodoItem>();
var nextId = 1;
var isRunning = true;

while (isRunning)
{
   Console.WriteLine();
   Console.WriteLine("=== Todo CLI ===");
   Console.WriteLine(" 1)List\n 2)Add\n 3)Complete\n 4)Delete\n 5)Update\n 6)Quit");
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

void ListItems()
{
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

void AddItem()
{
    Console.Write("Title: ");
    var title = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(title))
    {
        Console.WriteLine("A Title is required.");
        return;
    }

    Console.Write("Priority (Low/Medium/High), default is Medium: ");
    var priorityInput = Console.ReadLine()?.Trim();
    var priority = Enum.TryParse<Priority>(priorityInput, true, out var parsedPriority)
        ? parsedPriority
        : Priority.Medium;

    items.Add(new TodoItem
    {
        Id = nextId++,
        Title = title,
        Priority = priority
    });
    Console.WriteLine($"Added {title} with priority {priority}.");
}

void CompleteItem()
{
    if (!TryReadId(out var id)) return;

    var item = items.Find(i => i.Id == id);
    if (item == null)
    {
        Console.WriteLine($"No item found with ID {id}.");
        return;
    }

    item.IsDone = true;
    Console.WriteLine($"Completed item {id}: {item.Title}");
}

void UpdateItem()
{
    if (!TryReadId(out var id)) return;

    var item = items.Find(i => i.Id == id);
    if (item == null)
    {
        Console.WriteLine($"No item found with ID {id}.");
        return;
    }

    Console.Write($"New Title (leave empty to keep '{item.Title}'): ");
    var newTitle = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(newTitle))
    {
        item.Title = newTitle;
    }

    Console.Write($"New Priority (Low/Medium/High, leave empty to keep '{item.Priority}'): ");
    var priorityInput = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(priorityInput) && Enum.TryParse<Priority>(priorityInput, true, out var newPriority))
    {
        item.Priority = newPriority;
    }

    Console.WriteLine($"Updated item {id}: {item.Title} ({item.Priority})");
}

void DeleteItem()
{
    if (!TryReadId(out var id)) return;

    var removed = items.RemoveAll(i => i.Id == id);
    Console.WriteLine(removed > 0 ? $"Deleted item {id}." : $"No item found with ID {id}.");
}

bool TryReadId(out int id)
{
    Console.Write("Enter the ID of the item: ");
    var input = Console.ReadLine()?.Trim();

    if (!int.TryParse(input, out id))
    {
        Console.WriteLine("Invalid ID. Please enter a valid number.");
        return false;
    }

    return true;
}