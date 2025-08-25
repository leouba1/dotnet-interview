using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/todolist/{todolistId}/todoitems")]
    [ApiController]
    public class TodoItemController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/todolist/{id}/todoitems
        [HttpGet]
        public async Task<ActionResult<IList<TodoItemDto>>> GetTodoItems(long todolistId)
        {
            return Ok(await _context.TodoItems
                .Where(item => item.TodoListId == todolistId)
                .Select(item => new TodoItemDto
                {
                    Description = item.Description,
                    IsCompleted = item.IsCompleted
                })
                .ToListAsync());
        }

        // GET: api/todolist/{id}/todoitems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDto>> GetTodoItem(long todolistId, long id)
        {
            var todoItem = await _context.TodoItems
                .Where(x => x.TodoListId == todolistId && x.Id == id)
                .Select(x => new TodoItemDto
                {
                    Description = x.Description,
                    IsCompleted = x.IsCompleted
                })
                .FirstOrDefaultAsync();

            if (todoItem == null)
            {
                return NotFound();
            }

            return Ok(todoItem);
        }

        // PUT: api/todoitems/5
        // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult> PutTodoItem(long todolistId, long id, UpdateTodoItem payload)
        {
            var todoItem = await _context.TodoItems.FirstOrDefaultAsync(x => x.Id == id && x.TodoListId == todolistId);
            if (todoItem == null)
                return NotFound();

            if (payload.Description is not null)
                todoItem.Description = payload.Description;

            if (payload.IsCompleted.HasValue)
                todoItem.IsCompleted = payload.IsCompleted.Value;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/todoitems
        // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItemDto>> PostTodoItem(long todolistId, CreateTodoItem payload)
        {
            var todoList = await _context.TodoList.FindAsync(todolistId);
            if (todoList == null)
                return NotFound();

            var todoItem = new TodoItem
            {
                TodoListId = todolistId,
                Description = payload.Description,
                IsCompleted = payload.IsCompleted
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

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
            var todoItem = await _context.TodoItems
                .FirstOrDefaultAsync(x => x.Id == id && x.TodoListId == todolistId);

            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
