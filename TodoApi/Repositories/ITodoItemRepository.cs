using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITodoItemRepository
{
    Task<IList<TodoItem>> GetByListIdAsync(long listId, string? search = null, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    Task<TodoItem?> GetAsync(long listId, long id, bool track = false, CancellationToken cancellationToken = default);

    Task AddAsync(TodoItem item, CancellationToken cancellationToken = default);

    Task RemoveAsync(TodoItem item, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}