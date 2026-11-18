using TodoCli;

var path = Environment.GetEnvironmentVariable("TODO_FILE") ?? "todos.json";

ITodoRepository repository = new LoggingTodoRepository(new JsonTodoRepository(path));
var app = new TodoApp(repository);

await app.RunAsync();