using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models;

public class TodoList : IAuditable
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public List<TodoItem> TodoItems { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [NotMapped]
    public int ItemCount { get; set; }
}