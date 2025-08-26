using Microsoft.AspNetCore.Mvc;
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TodoListDto>))]
    public async Task<ActionResult<IList<TodoListDto>>> GetTodoLists(
        [FromQuery] bool includeItems = false,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving todo lists");
        var lists = await _repository.GetAllAsync(includeItems, search, page, pageSize, cancellationToken);
        var dtos = lists.Select(list => list.ToDto(includeItems)).ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// Retrieves a specific todo list.
    /// </summary>
    /// <param name="id">The identifier of the todo list.</param>
    /// <returns>The requested todo list.</returns>
    /// <response code="200">The requested todo list.</response>
    /// <response code="404">The todo list was not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TodoListDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoListDto>> GetTodoList(long id, CancellationToken cancellationToken = default)
    {
        if (await _repository.GetAsync(id, cancellationToken: cancellationToken) is not { } todoList)
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
    /// <response code="200">The todo list was updated successfully.</response>
    /// <response code="404">The todo list was not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TodoListDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PutTodoList(long id, UpdateTodoList payload, CancellationToken cancellationToken = default)
    {
        if (await _repository.GetAsync(id, track: true, cancellationToken: cancellationToken) is not { } todoList)
        {
            _logger.LogWarning("Todo list {Id} not found", id);
            return NotFound();
        }

        payload.UpdateModel(todoList);
        await _repository.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Todo list {Id} updated", id);

        return Ok(todoList.ToDto());
    }

    /// <summary>
    /// Creates a new todo list.
    /// </summary>
    /// <param name="payload">Data for the new list.</param>
    /// <returns>The created todo list.</returns>
    /// <response code="201">The todo list was created successfully.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TodoListDto))]
    public async Task<ActionResult<TodoListDto>> PostTodoList(CreateTodoList payload, CancellationToken cancellationToken = default)
    {
        var todoList = payload.ToModel();

        await _repository.AddAsync(todoList, cancellationToken);
        _logger.LogInformation("Todo list {Id} created", todoList.Id);

        return CreatedAtAction(nameof(GetTodoList), new { id = todoList.Id }, todoList.ToDto());
    }

    /// <summary>
    /// Deletes a todo list.
    /// </summary>
    /// <param name="id">The identifier of the todo list.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">The todo list was deleted.</response>
    /// <response code="404">The todo list was not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteTodoList(long id, CancellationToken cancellationToken = default)
    {
        if (await _repository.GetAsync(id, track: true, cancellationToken: cancellationToken) is not { } todoList)
        {
            _logger.LogWarning("Todo list {Id} not found", id);
            return NotFound();
        }

        await _repository.RemoveAsync(todoList, cancellationToken);
        _logger.LogInformation("Todo list {Id} deleted", id);

        return NoContent();
    }
}