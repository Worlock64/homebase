using TodoCli;

ITodoRepository repository = new LoggingTodoRepository(new InMemoryTodoRepository());
var app = new TodoApp(repository);
app.Run();