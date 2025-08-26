namespace TodoApi.Dtos.TodoItems;

public class UpdateTodoItem
{
    public string? Description { get; set; }
    public bool? IsCompleted { get; set; }
}