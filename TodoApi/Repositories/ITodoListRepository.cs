using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITodoListRepository
{
    Task<IList<TodoList>> GetAllAsync();
    Task<TodoList?> GetAsync(long id);
    Task AddAsync(TodoList list);
    Task RemoveAsync(TodoList list);
    Task SaveChangesAsync();
}
