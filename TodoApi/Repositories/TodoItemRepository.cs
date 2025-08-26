using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class TodoItemRepository(TodoContext _context) : ITodoItemRepository
{
    public async Task<IList<TodoItem>> GetByListIdAsync(long listId)
    {
        return await _context.TodoItems
            .Where(i => i.TodoListId == listId)
            .AsNoTracking()
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
