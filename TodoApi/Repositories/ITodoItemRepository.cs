using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITodoItemRepository
{
    Task<IList<TodoItem>> GetByListIdAsync(long listId);
    Task<TodoItem?> GetAsync(long listId, long id, bool track = false);
    Task AddAsync(TodoItem item);
    Task RemoveAsync(TodoItem item);
    Task SaveChangesAsync();
}
