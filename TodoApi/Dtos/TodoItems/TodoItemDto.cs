using System;

namespace TodoApi.Dtos.TodoItems;

public class TodoItemDto
{
    public long Id { get; set; }
    public required string Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}