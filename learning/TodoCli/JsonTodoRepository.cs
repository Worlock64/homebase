using System.Text.Json;
using System.Text.Json.Serialization;

namespace TodoCli;

public class JsonTodoRepository : ITodoRepository
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = {new JsonStringEnumConverter()}
    };

    private readonly string _path;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private List<TodoItem>? _items;
    private int _nextId = 1;

    public JsonTodoRepository(string path)
    {
        _path = Path.GetFullPath(path);
    }

    public async Task<IReadOnlyList<TodoItem>> GetAllAsync()
    {
        await _lock.WaitAsync();
        try
        {
            await EnsureLoadedAsync();
            return _items!.ToList();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<TodoItem?> GetByIdAsync(int id)
    {
        await _lock.WaitAsync();
        try
        {
            await EnsureLoadedAsync();
            return _items!.Find(i => i.Id == id);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<TodoItem> AddAsync(TodoItem item)
    {
        await _lock.WaitAsync();
        try
        {
            await EnsureLoadedAsync();
            item.Id = _nextId++;
            _items!.Add(item);
            await SaveAsync();
            return item;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> UpdateAsync(TodoItem item)
    {
        await _lock.WaitAsync();
        try
        {
            await EnsureLoadedAsync();
            var index = _items!.FindIndex(i => i.Id == item.Id);
            if (index < 0) return false;

            _items[index] = item;
            await SaveAsync();
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await _lock.WaitAsync();
        try
        {
            await EnsureLoadedAsync();
            var removed = _items!.RemoveAll(i => i.Id == id) > 0;
            if (removed) await SaveAsync();
            return removed;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task EnsureLoadedAsync()
    {
        if (_items is not null) return;

        if (File.Exists(_path))
        {
            await using var stream = File.OpenRead(_path);
            _items = await JsonSerializer.DeserializeAsync<List<TodoItem>>(stream, Options)
                ?? new List<TodoItem>();
        }
        else
        {
            _items = new List<TodoItem>();
        }

        _nextId = _items.Count == 0 ? 1 : _items.Max(i=>i.Id) + 1;
    }

    private async Task SaveAsync()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);

        var temp = _path + ".tmp";
        await using (var stream = File.Create(temp))
        {
            await JsonSerializer.SerializeAsync(stream, _items, Options);
        }

        File.Move(temp, _path, overwrite: true);
    }
}