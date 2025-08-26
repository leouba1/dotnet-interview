using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITodoListRepository
{
    Task<IList<TodoList>> GetAllAsync(bool includeItems = false);
    Task<TodoList?> GetAsync(long id, bool track = false);
    Task AddAsync(TodoList list);
    Task RemoveAsync(TodoList list);
    Task SaveChangesAsync();
}
