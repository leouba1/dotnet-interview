using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TodoApi.Dtos.TodoLists;
using TodoApi.Mappers;
using TodoApi.Repositories;

namespace TodoApi.Controllers;

/// <summary>
/// API for managing todo lists.
/// </summary>
[Route("api/todolists")]
[ApiController]
public class TodoListsController(
    ITodoListRepository _repository,
    ILogger<TodoListsController> _logger
) : ControllerBase
{
    /// <summary>
    /// Retrieves todo lists with optional pagination and search.
    /// </summary>
    /// <param name="includeItems">Whether to include todo items in the response.</param>
    /// <param name="search">Optional search term to filter lists.</param>
    /// <param name="page">The page number for pagination.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A list of todo lists.</returns>
    [HttpGet]
    public async Task<ActionResult<IList<TodoListDto>>> GetTodoLists(
        [FromQuery] bool includeItems = false,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Retrieving todo lists");
        var lists = await _repository.GetAllAsync(includeItems, search, page, pageSize);
        var dtos = lists.Select(list => list.ToDto(includeItems)).ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// Retrieves a specific todo list.
    /// </summary>
    /// <param name="id">The identifier of the todo list.</param>
    /// <returns>The requested todo list.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListDto>> GetTodoList(long id)
    {
        if (await _repository.GetAsync(id) is not { } todoList)
        {
            _logger.LogWarning("Todo list {Id} not found", id);
            return NotFound();
        }

        return Ok(todoList.ToDto());
    }

    /// <summary>
    /// Updates a todo list.
    /// </summary>
    /// <param name="id">The identifier of the todo list.</param>
    /// <param name="payload">Updated fields for the todo list.</param>
    /// <returns>The updated todo list.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult> PutTodoList(long id, UpdateTodoList payload)
    {
        if (await _repository.GetAsync(id, track: true) is not { } todoList)
        {
            _logger.LogWarning("Todo list {Id} not found", id);
            return NotFound();
        }

        payload.UpdateModel(todoList);
        await _repository.SaveChangesAsync();
        _logger.LogInformation("Todo list {Id} updated", id);

        return Ok(todoList.ToDto());
    }

    /// <summary>
    /// Creates a new todo list.
    /// </summary>
    /// <param name="payload">Data for the new list.</param>
    /// <returns>The created todo list.</returns>
    [HttpPost]
    public async Task<ActionResult<TodoListDto>> PostTodoList(CreateTodoList payload)
    {
        var todoList = payload.ToModel();

        await _repository.AddAsync(todoList);
        _logger.LogInformation("Todo list {Id} created", todoList.Id);

        return CreatedAtAction(nameof(GetTodoList), new { id = todoList.Id }, todoList.ToDto());
    }

    /// <summary>
    /// Deletes a todo list.
    /// </summary>
    /// <param name="id">The identifier of the todo list.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTodoList(long id)
    {
        if (await _repository.GetAsync(id, track: true) is not { } todoList)
        {
            _logger.LogWarning("Todo list {Id} not found", id);
            return NotFound();
        }

        await _repository.RemoveAsync(todoList);
        _logger.LogInformation("Todo list {Id} deleted", id);

        return NoContent();
    }

}
