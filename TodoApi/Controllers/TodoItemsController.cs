using Microsoft.AspNetCore.Mvc;
using TodoApi.Dtos.TodoItems;
using TodoApi.Mappers;
using TodoApi.Repositories;

namespace TodoApi.Controllers;

/// <summary>
/// API for managing todo items within a todo list.
/// </summary>
[Route("api/todolist/{todolistId}/todoitems")]
[ApiController]
public class TodoItemsController(
    ITodoItemRepository _itemRepository,
    ITodoListRepository _listRepository,
    ILogger<TodoItemsController> _logger
) : ControllerBase
{
    /// <summary>
    /// Retrieves items for the specified todo list.
    /// </summary>
    /// <param name="todolistId">The identifier of the todo list.</param>
    /// <param name="search">Optional search term to filter items.</param>
    /// <param name="page">The page number for pagination.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A list of todo items.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TodoItemDto>))]
    public async Task<ActionResult<IList<TodoItemDto>>> GetTodoItems(
        long todolistId,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Retrieving items for list {ListId}", todolistId);
        var items = await _itemRepository.GetByListIdAsync(todolistId, search, page, pageSize);
        return Ok(items.Select(item => item.ToDto()).ToList());
    }

    /// <summary>
    /// Retrieves a specific todo item from the specified list.
    /// </summary>
    /// <param name="todolistId">The identifier of the todo list.</param>
    /// <param name="id">The identifier of the todo item.</param>
    /// <returns>The requested todo item.</returns>
    /// <response code="200">The requested todo item.</response>
    /// <response code="404">The todo item was not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TodoItemDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoItemDto>> GetTodoItem(long todolistId, long id)
    {
        if (await _itemRepository.GetAsync(todolistId, id) is not { } todoItem)
        {
            _logger.LogWarning("Todo item {ItemId} for list {ListId} not found", id, todolistId);
            return NotFound();
        }

        return Ok(todoItem.ToDto());
    }

    /// <summary>
    /// Updates a todo item in the specified list.
    /// </summary>
    /// <param name="todolistId">The identifier of the todo list.</param>
    /// <param name="id">The identifier of the todo item.</param>
    /// <param name="payload">Updated fields for the todo item.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">The todo item was updated successfully.</response>
    /// <response code="404">The todo item was not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PutTodoItem(long todolistId, long id, UpdateTodoItem payload)
    {
        if (await _itemRepository.GetAsync(todolistId, id, track: true) is not { } todoItem)
        {
            _logger.LogWarning("Todo item {ItemId} for list {ListId} not found", id, todolistId);
            return NotFound();
        }

        payload.UpdateModel(todoItem);

        await _itemRepository.SaveChangesAsync();
        _logger.LogInformation("Todo item {ItemId} updated", id);

        return NoContent();
    }

    /// <summary>
    /// Creates a new todo item in the specified list.
    /// </summary>
    /// <param name="todolistId">The identifier of the todo list.</param>
    /// <param name="payload">Data for the new todo item.</param>
    /// <returns>The created todo item.</returns>
    /// <response code="201">The todo item was created successfully.</response>
    /// <response code="404">The parent todo list was not found.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TodoItemDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoItemDto>> PostTodoItem(long todolistId, CreateTodoItem payload)
    {
        if (await _listRepository.GetAsync(todolistId) is not { } todoList)
        {
            _logger.LogWarning("Todo list {ListId} not found when creating item", todolistId);
            return NotFound();
        }

        var todoItem = payload.ToModel(todolistId);

        await _itemRepository.AddAsync(todoItem);
        _logger.LogInformation("Todo item {ItemId} created in list {ListId}", todoItem.Id, todolistId);

        return CreatedAtAction(nameof(GetTodoItem), new { todolistId, id = todoItem.Id }, todoItem.ToDto());
    }

    /// <summary>
    /// Deletes a todo item from the specified list.
    /// </summary>
    /// <param name="todolistId">The identifier of the todo list.</param>
    /// <param name="id">The identifier of the todo item.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">The todo item was deleted.</response>
    /// <response code="404">The todo item was not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteTodoItem(long todolistId, long id)
    {
        if (await _itemRepository.GetAsync(todolistId, id, track: true) is not { } todoItem)
        {
            _logger.LogWarning("Todo item {ItemId} for list {ListId} not found", id, todolistId);
            return NotFound();
        }

        await _itemRepository.RemoveAsync(todoItem);
        _logger.LogInformation("Todo item {ItemId} deleted from list {ListId}", id, todolistId);

        return NoContent();
    }
}