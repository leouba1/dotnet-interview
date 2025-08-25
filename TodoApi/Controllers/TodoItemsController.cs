using Microsoft.AspNetCore.Mvc;
using TodoApi.Dtos;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Controllers
{
    [Route("api/todolist/{todolistId}/todoitems")]
    [ApiController]
    public class TodoItemController : ControllerBase
    {
        private readonly ITodoItemRepository _items;
        private readonly ITodoListRepository _lists;

        public TodoItemController(ITodoItemRepository items, ITodoListRepository lists)
        {
            _items = items;
            _lists = lists;
        }

        // GET: api/todolist/{id}/todoitems
        [HttpGet]
        public async Task<ActionResult<IList<TodoItemDto>>> GetTodoItems(long todolistId)
        {
            var items = await _items.GetByListIdAsync(todolistId);
            return Ok(items.Select(item => new TodoItemDto
            {
                Description = item.Description,
                IsCompleted = item.IsCompleted
            }).ToList());
        }

        // GET: api/todolist/{id}/todoitems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDto>> GetTodoItem(long todolistId, long id)
        {
            var todoItem = await _items.GetAsync(todolistId, id);

            if (todoItem == null)
            {
                return NotFound();
            }

            var dto = new TodoItemDto
            {
                Description = todoItem.Description,
                IsCompleted = todoItem.IsCompleted
            };

            return Ok(dto);
        }

        // PUT: api/todoitems/5
        // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult> PutTodoItem(long todolistId, long id, UpdateTodoItem payload)
        {
            var todoItem = await _items.GetAsync(todolistId, id);
            if (todoItem == null)
                return NotFound();

            if (payload.Description is not null)
                todoItem.Description = payload.Description;

            if (payload.IsCompleted.HasValue)
                todoItem.IsCompleted = payload.IsCompleted.Value;

            await _items.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/todoitems
        // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItemDto>> PostTodoItem(long todolistId, CreateTodoItem payload)
        {
            var todoList = await _lists.GetAsync(todolistId);
            if (todoList == null)
                return NotFound();

            var todoItem = new TodoItem
            {
                TodoListId = todolistId,
                Description = payload.Description,
                IsCompleted = payload.IsCompleted
            };

            await _items.AddAsync(todoItem);

            var dto = new TodoItemDto
            {
                Description = todoItem.Description,
                IsCompleted = todoItem.IsCompleted
            };

            return CreatedAtAction(nameof(GetTodoItem), new { todolistId, id = todoItem.Id }, dto);
        }

        // DELETE: api/todolist/{todolistId}/todoitems/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodoItem(long todolistId, long id)
        {
            var todoItem = await _items.GetAsync(todolistId, id);

            if (todoItem == null)
            {
                return NotFound();
            }

            await _items.RemoveAsync(todoItem);

            return NoContent();
        }
    }
}
