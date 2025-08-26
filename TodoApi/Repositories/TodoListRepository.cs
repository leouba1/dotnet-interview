using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class TodoListRepository(TodoContext _context) : ITodoListRepository
{
    public async Task<IList<TodoList>> GetAllAsync(bool includeItems = false, string? search = null, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = _context.TodoList.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(l =>
                EF.Functions.Like(l.Name, $"%{search}%") ||
                l.TodoItems.Any(i => EF.Functions.Like(i.Description, $"%{search}%")));

        if (includeItems)
            query = query.Include(l => l.TodoItems);

        query = query.OrderBy(l => l.Id)
                     .Skip((page - 1) * pageSize)
                     .Take(pageSize);

        return await query
            .Select(l => new TodoList
            {
                Id = l.Id,
                Name = l.Name,
                ItemCount = l.TodoItems.Count,
                CreatedAt = l.CreatedAt,
                UpdatedAt = l.UpdatedAt,
                TodoItems = includeItems ? l.TodoItems : new List<TodoItem>()
            })
            .ToListAsync(cancellationToken);
    }

    public Task<TodoList?> GetAsync(long id, bool track = false, CancellationToken cancellationToken = default)
    {
        IQueryable<TodoList> query = _context.TodoList
            .Include(l => l.TodoItems);

        if (!track)
            query = query.AsNoTracking();

        return query.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task AddAsync(TodoList list, CancellationToken cancellationToken = default)
    {
        _context.TodoList.Add(list);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(TodoList list, CancellationToken cancellationToken = default)
    {
        _context.TodoList.Remove(list);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);
}