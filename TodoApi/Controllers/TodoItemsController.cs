using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TodoApi.Dtos.TodoItems;
using TodoApi.Mappers;
using TodoApi.Repositories;

namespace TodoApi.Controllers;

[Route("api/todolist/{todolistId}/todoitems")]
[ApiController]
public class TodoItemsController(
    ITodoItemRepository _itemRepository,
    ITodoListRepository _listRepository,
    ILogger<TodoItemsController> _logger
) : ControllerBase
{
    // GET: api/todolist/{id}/todoitems
    [HttpGet]
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

    // GET: api/todolist/{id}/todoitems/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDto>> GetTodoItem(long todolistId, long id)
    {
        if (await _itemRepository.GetAsync(todolistId, id) is not { } todoItem)
        {
            _logger.LogWarning("Todo item {ItemId} for list {ListId} not found", id, todolistId);
            return NotFound();
        }

        return Ok(todoItem.ToDto());
    }

    // PUT: api/todoitems/5
    // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
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

    // POST: api/todoitems
    // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
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

    // DELETE: api/todolist/{todolistId}/todoitems/{id}
    [HttpDelete("{id}")]
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
