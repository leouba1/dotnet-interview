using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITodoListRepository
{
    /// <summary>
    /// Retrieves todo lists with optional filtering and pagination.
    /// </summary>
    /// <param name="includeItems">Whether to include list items in the result.</param>
    /// <param name="search">Optional term to search by list name or item description.</param>
    /// <param name="page">The 1-based page index.</param>
    /// <param name="pageSize">The number of records per page.</param>
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
