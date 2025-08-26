namespace TodoApi.Dtos.TodoItems;

public class TodoItemDto
{
    public required string Description { get; set; }
    public bool IsCompleted { get; set; }
}
