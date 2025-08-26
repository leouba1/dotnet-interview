using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITodoItemRepository
{
    Task<IList<TodoItem>> GetByListIdAsync(
        long listId,
        string? search = null,
        int page = 1,
        int pageSize = 10);
    Task<TodoItem?> GetAsync(long listId, long id, bool track = false);
    Task AddAsync(TodoItem item);
    Task RemoveAsync(TodoItem item);
    Task SaveChangesAsync();
}
