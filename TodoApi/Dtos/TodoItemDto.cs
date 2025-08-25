namespace TodoApi.Dtos;

public class TodoItemDto
{
    public required string Description { get; set; }
    public bool IsCompleted { get; set; }
}
