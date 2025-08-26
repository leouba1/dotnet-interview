using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITodoListRepository
{
    Task<IList<TodoList>> GetAllAsync(
        bool includeItems = false,
        string? search = null,
        int page = 1,
        int pageSize = 10);
    Task<TodoList?> GetAsync(long id, bool track = false);
    Task AddAsync(TodoList list);
    Task RemoveAsync(TodoList list);
    Task SaveChangesAsync();
}
