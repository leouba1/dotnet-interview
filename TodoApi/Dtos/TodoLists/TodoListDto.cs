using TodoApi.Dtos.TodoItems;
namespace TodoApi.Dtos.TodoLists;

public class TodoListDto
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public int ItemCount { get; set; }
    public IList<TodoItemDto> Items { get; set; } = new List<TodoItemDto>();
}
