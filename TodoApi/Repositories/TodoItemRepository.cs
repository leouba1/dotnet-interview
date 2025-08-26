using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class TodoItemRepository(TodoContext _context) : ITodoItemRepository
{
    public async Task<IList<TodoItem>> GetByListIdAsync(
        long listId,
        string? search = null,
        int page = 1,
        int pageSize = 10)
    {
        var query = _context.TodoItems
            .Where(i => i.TodoListId == listId)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(i => EF.Functions.Like(i.Description, $"%{search}%"));

        return await query
            .OrderBy(i => i.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public Task<TodoItem?> GetAsync(long listId, long id, bool track = false)
    {
        IQueryable<TodoItem> query = _context.TodoItems;

        if (!track)
            query = query.AsNoTracking();

        return query.FirstOrDefaultAsync(i => i.TodoListId == listId && i.Id == id);
    }

    public async Task AddAsync(TodoItem item)
    {
        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(TodoItem item)
    {
        _context.TodoItems.Remove(item);
        await _context.SaveChangesAsync();
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
