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
        public async Task<ActionResult<IList<TodoItem>>> GetTodoItems(long todolistId)
        {
            return Ok(await _context.TodoItems.Where(item => item.TodoListId == todolistId).ToListAsync());
        }

        // GET: api/todolist/{id}/todoitems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long todolistId, long id)
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
            var todoItem = await _context.TodoItems.FirstOrDefaultAsync(x => x.Id == id);
            if (todoItem == null)
                return NotFound();

            todoItem.Description = payload.Description;
            todoItem.IsCompleted = payload.IsCompleted;

            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/todoitems
        // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(long todolistId, CreateTodoItem payload)
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

            return CreatedAtAction("PostTodoItem", new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/todolists/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
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
