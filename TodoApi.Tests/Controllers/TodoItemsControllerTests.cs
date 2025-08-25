using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Controllers;
using TodoApi.Models;

namespace TodoApi.Tests;

#nullable disable
public class TodoItemsControllerTests
{
    private DbContextOptions<TodoContext> DatabaseContextOptions()
    {
        return new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    private void PopulateDatabaseContext(TodoContext context)
    {
        context.TodoList.Add(new TodoList { Id = 1, Name = "List 1" });
        context.TodoList.Add(new TodoList { Id = 2, Name = "List 2" });
        context.TodoItems.Add(new TodoItem { Id = 1, TodoListId = 1, Description = "Item 1", IsCompleted = false });
        context.TodoItems.Add(new TodoItem { Id = 2, TodoListId = 2, Description = "Item 2", IsCompleted = false });
        context.SaveChanges();
    }

    [Fact]
    public async Task DeleteTodoItem_WhenItemBelongsToList_RemovesItem()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemController(context);

            var result = await controller.DeleteTodoItem(1, 1);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal(1, context.TodoItems.Count());
        }
    }

    [Fact]
    public async Task DeleteTodoItem_WhenItemDoesntBelongToList_ReturnsNotFound()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemController(context);

            var result = await controller.DeleteTodoItem(1, 2);

            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(2, context.TodoItems.Count());
        }
    }
}
