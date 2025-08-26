using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class TodoListRepository(TodoContext _context) : ITodoListRepository
{
    public async Task<IList<TodoList>> GetAllAsync()
    {
        return await _context.TodoList.AsNoTracking().ToListAsync();
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

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
