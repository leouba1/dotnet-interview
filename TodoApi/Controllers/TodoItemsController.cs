using Microsoft.AspNetCore.Mvc;
using TodoApi.Dtos.TodoItems;
using TodoApi.Mappers;
using TodoApi.Repositories;

namespace TodoApi.Controllers;

[Route("api/todolist/{todolistId}/todoitems")]
[ApiController]
public class TodoItemsController(ITodoItemRepository _itemRepository, ITodoListRepository _listRepository) : ControllerBase
{
    // GET: api/todolist/{id}/todoitems
    [HttpGet]
    public async Task<ActionResult<IList<TodoItemDto>>> GetTodoItems(long todolistId)
    {
        var items = await _itemRepository.GetByListIdAsync(todolistId);
        return Ok(items.Select(item => item.ToDto()).ToList());
    }

    // GET: api/todolist/{id}/todoitems/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDto>> GetTodoItem(long todolistId, long id)
    {
        var todoItem = await _itemRepository.GetAsync(todolistId, id);

        if (todoItem == null)
        {
            return NotFound();
        }

        return Ok(todoItem.ToDto());
    }

    // PUT: api/todoitems/5
    // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<ActionResult> PutTodoItem(long todolistId, long id, UpdateTodoItem payload)
    {
        var todoItem = await _itemRepository.GetAsync(todolistId, id, track: true);
        if (todoItem == null)
            return NotFound();

        payload.UpdateModel(todoItem);

        await _itemRepository.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/todoitems
    // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TodoItemDto>> PostTodoItem(long todolistId, CreateTodoItem payload)
    {
        var todoList = await _listRepository.GetAsync(todolistId);
        if (todoList == null)
            return NotFound();

        var todoItem = payload.ToModel(todolistId);

        await _itemRepository.AddAsync(todoItem);

        return CreatedAtAction(nameof(GetTodoItem), new { todolistId, id = todoItem.Id }, todoItem.ToDto());
    }

    // DELETE: api/todolist/{todolistId}/todoitems/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTodoItem(long todolistId, long id)
    {
        var todoItem = await _itemRepository.GetAsync(todolistId, id, track: true);

        if (todoItem == null)
        {
            return NotFound();
        }

        await _itemRepository.RemoveAsync(todoItem);

        return NoContent();
    }
}
