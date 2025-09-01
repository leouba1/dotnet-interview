using System;

namespace TodoApi.Models;

public class TodoItem : IAuditable
{
    public long Id { get; set; }
    public long TodoListId { get; set; }
    public string Description { get; set; } = null!;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}