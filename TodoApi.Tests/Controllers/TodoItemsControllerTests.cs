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

     [Fact]
    public async Task PutTodoItem_WithOnlyIsCompleted_UpdatesField()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemController(context);

            var payload = new Dtos.UpdateTodoItem { IsCompleted = true };

            var result = await controller.PutTodoItem(1, 1, payload);

            Assert.IsType<NoContentResult>(result);
            var item = await context.TodoItems.FindAsync(1L);
            Assert.NotNull(item);
            Assert.True(item.IsCompleted);
            Assert.Equal("Item 1", item.Description);
        }
    }
}
