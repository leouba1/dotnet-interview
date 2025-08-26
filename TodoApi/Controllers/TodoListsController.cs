using Microsoft.AspNetCore.Mvc;
using TodoApi.Dtos;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Controllers
{
    [Route("api/todolists")]
    [ApiController]
    public class TodoListsController : ControllerBase
    {
        private readonly ITodoListRepository _repository;

        public TodoListsController(ITodoListRepository repository)
        {
            _repository = repository;
        }

        // GET: api/todolists
        [HttpGet]
        public async Task<ActionResult<IList<TodoListDto>>> GetTodoLists()
        {
            var lists = await _repository.GetAllAsync();
            var dtos = lists.Select(list => new TodoListDto
            {
                Id = list.Id,
                Name = list.Name,
                Items = list.TodoItems.Select(item => new TodoItemDto
                {
                    Description = item.Description,
                    IsCompleted = item.IsCompleted
                }).ToList()
            }).ToList();

            return Ok(dtos);
        }

        // GET: api/todolists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoListDto>> GetTodoList(long id)
        {
            var todoList = await _repository.GetAsync(id);

            if (todoList == null)
            {
                return NotFound();
            }

            var dto = new TodoListDto
            {
                Id = todoList.Id,
                Name = todoList.Name,
                Items = todoList.TodoItems.Select(item => new TodoItemDto
                {
                    Description = item.Description,
                    IsCompleted = item.IsCompleted
                }).ToList()
            };

            return Ok(dto);
        }

        // PUT: api/todolists/5
        // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult> PutTodoList(long id, UpdateTodoList payload)
        {
            var todoList = await _repository.GetAsync(id);

            if (todoList == null)
            {
                return NotFound();
            }

            todoList.Name = payload.Name;
            await _repository.SaveChangesAsync();

            return Ok(todoList);
        }

        // POST: api/todolists
        // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoListDto>> PostTodoList(CreateTodoList payload)
        {
            var todoList = new TodoList { Name = payload.Name };

            await _repository.AddAsync(todoList);

            var dto = new TodoListDto
            {
                Name = todoList.Name
            };

            return CreatedAtAction(nameof(GetTodoList), new { id = todoList.Id }, dto);
        }

        // DELETE: api/todolists/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodoList(long id)
        {
            var todoList = await _repository.GetAsync(id);
            if (todoList == null)
            {
                return NotFound();
            }

            await _repository.RemoveAsync(todoList);

            return NoContent();
        }

    }
}
