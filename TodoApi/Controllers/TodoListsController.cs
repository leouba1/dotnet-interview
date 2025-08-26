using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TodoApi.Dtos.TodoLists;
using TodoApi.Mappers;
using TodoApi.Repositories;

namespace TodoApi.Controllers;

[Route("api/todolists")]
[ApiController]
public class TodoListsController(
    ITodoListRepository _repository,
    ILogger<TodoListsController> _logger
) : ControllerBase
{
    // GET: api/todolists
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

    // GET: api/todolists/5
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

    // PUT: api/todolists/5
    // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

    // POST: api/todolists
    // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TodoListDto>> PostTodoList(CreateTodoList payload)
    {
        var todoList = payload.ToModel();

        await _repository.AddAsync(todoList);
        _logger.LogInformation("Todo list {Id} created", todoList.Id);

        return CreatedAtAction(nameof(GetTodoList), new { id = todoList.Id }, todoList.ToDto());
    }

    // DELETE: api/todolists/5
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
