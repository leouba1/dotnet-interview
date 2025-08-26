using System.ComponentModel.DataAnnotations;

namespace TodoApi.Dtos.TodoItems;

public class CreateTodoItem
{
    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; } = null!;
    public bool IsCompleted { get; set; }
}
