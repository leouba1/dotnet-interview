namespace TodoApi.Dtos.TodoItems;

public class CreateTodoItem
{
    public required string Description { get; set; }
    public bool IsCompleted { get; set; }
}