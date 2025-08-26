using System.ComponentModel.DataAnnotations;

namespace TodoApi.Dtos.TodoLists;

public class CreateTodoList
{
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = null!;
}
