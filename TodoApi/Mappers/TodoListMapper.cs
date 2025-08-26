using TodoApi.Dtos.TodoLists;
using TodoApi.Models;

namespace TodoApi.Mappers;

public static class TodoListMapper
{
    public static TodoListDto ToDto(this TodoList list)
    {
        return new TodoListDto
        {
            Id = list.Id,
            Name = list.Name,
            Items = list.TodoItems.Select(item => item.ToDto()).ToList()
        };
    }

    public static TodoListSummaryDto ToSummaryDto(this TodoList list)
    {
        return new TodoListSummaryDto
        {
            Id = list.Id,
            Name = list.Name,
            ItemCount = list.ItemCount
        };
    }

    public static TodoList ToModel(this CreateTodoList dto)
    {
        return new TodoList
        {
            Name = dto.Name
        };
    }

    public static void UpdateModel(this UpdateTodoList dto, TodoList model)
    {
        model.Name = dto.Name;
    }
}
