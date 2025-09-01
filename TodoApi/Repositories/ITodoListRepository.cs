using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITodoListRepository
{
    Task<IList<TodoList>> GetAllAsync(bool includeItems = false, string? search = null, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    Task<TodoList?> GetAsync(long id, bool track = false, CancellationToken cancellationToken = default);

    Task AddAsync(TodoList list, CancellationToken cancellationToken = default);

    Task RemoveAsync(TodoList list, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}