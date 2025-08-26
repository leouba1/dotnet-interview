using System;
using TodoApi.Dtos.TodoLists;
using TodoApi.Dtos.TodoItems;
using TodoApi.Models;

namespace TodoApi.Mappers;

public static class TodoListMapper
{
    public static TodoListDto ToDto(this TodoList list, bool includeItems = true)
    {
        return new TodoListDto
        {
            Id = list.Id,
            Name = list.Name,
            ItemCount = list.ItemCount != 0 ? list.ItemCount : list.TodoItems.Count,
            CreatedAt = list.CreatedAt,
            UpdatedAt = list.UpdatedAt,
            Items = includeItems ? list.TodoItems.Select(item => item.ToDto()).ToList() : new List<TodoItemDto>()
        };
    }

    public static TodoList ToModel(this CreateTodoList dto)
    {
        return new TodoList
        {
            Name = dto.Name,
        };
    }

    public static void UpdateModel(this UpdateTodoList dto, TodoList model)
    {
        model.Name = dto.Name;
    }
}