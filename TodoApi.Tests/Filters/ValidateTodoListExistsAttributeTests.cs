using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TodoApi.Filters;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Tests.Filters;

public class ValidateTodoListExistsAttributeTests
{
    private DbContextOptions<TodoContext> Options()
        => new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    [Fact]
    public async Task OnActionExecutionAsync_ListMissing_SetsNotFoundResult()
    {
        using var context = new TodoContext(Options());
        var repository = new TodoListRepository(context);
        var filter = new ValidateTodoListExistsAttribute(repository, NullLogger<ValidateTodoListExistsAttribute>.Instance);

        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var executingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { { "todolistId", 42L } },
            controller: null
        );

        await filter.OnActionExecutionAsync(executingContext, () => Task.FromResult<ActionExecutedContext>(null!));

        Assert.IsType<NotFoundResult>(executingContext.Result);
    }

    [Fact]
    public async Task OnActionExecutionAsync_ListExists_InvokesNext()
    {
        using var context = new TodoContext(Options());
        context.TodoList.Add(new TodoList { Id = 1, Name = "List" });
        context.SaveChanges();

        var repository = new TodoListRepository(context);
        var filter = new ValidateTodoListExistsAttribute(repository, NullLogger<ValidateTodoListExistsAttribute>.Instance);

        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var executingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { { "todolistId", 1L } },
            controller: null
        );

        var called = false;
        await filter.OnActionExecutionAsync(executingContext, () =>
        {
            called = true;
            return Task.FromResult(new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), controller: null));
        });

        Assert.True(called);
        Assert.Null(executingContext.Result);
    }
}
