namespace TodoApi.Dtos.TodoLists;

/// <summary>
/// Represents a lightweight view of a todo list including item count.
/// </summary>
public class TodoListSummaryDto
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public int ItemCount { get; set; }
}

