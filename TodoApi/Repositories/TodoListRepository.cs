using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class TodoListRepository(TodoContext _context) : ITodoListRepository
{
    public async Task<IList<TodoList>> GetAllAsync(bool includeItems = false, string? search = null, int page = 1, int pageSize = 10)
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
                TodoItems = includeItems ? l.TodoItems : new List<TodoItem>()
            })
            .ToListAsync();
    }

    public Task<TodoList?> GetAsync(long id, bool track = false)
    {
        IQueryable<TodoList> query = _context.TodoList
            .Include(l => l.TodoItems);

        if (!track)
            query = query.AsNoTracking();

        return query.FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task AddAsync(TodoList list)
    {
        _context.TodoList.Add(list);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(TodoList list)
    {
        _context.TodoList.Remove(list);
        await _context.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}