using System;
using TodoApi.Dtos.TodoItems;
using TodoApi.Models;

namespace TodoApi.Mappers;

public static class TodoItemMapper
{
    public static TodoItemDto ToDto(this TodoItem item)
    {
        return new TodoItemDto
        {
            Id = item.Id,
            Description = item.Description,
            IsCompleted = item.IsCompleted,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
        };
    }

    public static TodoItem ToModel(this CreateTodoItem dto, long todoListId)
    {
        return new TodoItem
        {
            TodoListId = todoListId,
            Description = dto.Description,
            IsCompleted = dto.IsCompleted,
        };
    }

    public static void UpdateModel(this UpdateTodoItem dto, TodoItem model)
    {
        if (dto.Description is not null)
        {
            model.Description = dto.Description;
        }

        if (dto.IsCompleted.HasValue)
        {
            model.IsCompleted = dto.IsCompleted.Value;
        }
    }
}