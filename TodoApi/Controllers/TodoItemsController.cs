using Microsoft.AspNetCore.Mvc;
using TodoApi.Dtos;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Controllers
{
    [Route("api/todolist/{todolistId}/todoitems")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemRepository _itemRepository;
        private readonly ITodoListRepository _listRepository;

        public TodoItemsController(ITodoItemRepository items, ITodoListRepository lists)
        {
            _itemRepository = items;
            _listRepository = lists;
        }

        // GET: api/todolist/{id}/todoitems
        [HttpGet]
        public async Task<ActionResult<IList<TodoItemDto>>> GetTodoItems(long todolistId)
        {
            var items = await _itemRepository.GetByListIdAsync(todolistId);
            return Ok(items.Select(item => new TodoItemDto
            {
                Id = item.Id,
                Description = item.Description,
                IsCompleted = item.IsCompleted
            }).ToList());
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

            var dto = new TodoItemDto
            {
                Id = todoItem.Id,
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
            var todoItem = await _itemRepository.GetAsync(todolistId, id);
            if (todoItem == null)
                return NotFound();

            if (payload.Description is not null)
                todoItem.Description = payload.Description;

            if (payload.IsCompleted.HasValue)
                todoItem.IsCompleted = payload.IsCompleted.Value;

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

            var todoItem = new TodoItem
            {
                TodoListId = todolistId,
                Description = payload.Description,
                IsCompleted = payload.IsCompleted
            };

            await _itemRepository.AddAsync(todoItem);

            var dto = new TodoItemDto
            {
                Id = todoItem.Id,
                Description = todoItem.Description,
                IsCompleted = todoItem.IsCompleted
            };

            return CreatedAtAction(nameof(GetTodoItem), new { todolistId, id = todoItem.Id }, dto);
        }

        // DELETE: api/todolist/{todolistId}/todoitems/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodoItem(long todolistId, long id)
        {
            var todoItem = await _itemRepository.GetAsync(todolistId, id);

            if (todoItem == null)
            {
                return NotFound();
            }

            await _itemRepository.RemoveAsync(todoItem);

            return NoContent();
        }
    }
}
