using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TodoApi.Repositories;

namespace TodoApi.Filters;

/// <summary>
/// Ensures the requested todo list exists before executing the action.
/// </summary>
public class ValidateTodoListExistsAttribute(
    ITodoListRepository _repository,
    ILogger<ValidateTodoListExistsAttribute> _logger
) : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.TryGetValue("todolistId", out var value) && value is long listId)
        {
            if (await _repository.GetAsync(listId, cancellationToken: context.HttpContext.RequestAborted) is null)
            {
                _logger.LogWarning("Todo list {ListId} not found", listId);
                context.Result = new NotFoundResult();
                return;
            }
        }

        await next();
    }
}
