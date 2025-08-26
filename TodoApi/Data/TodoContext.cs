using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options) { }

    public DbSet<TodoList> TodoList { get; set; } = default!;
    public DbSet<TodoItem> TodoItems { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>()
            .Property(t => t.Description)
            .IsRequired();

        modelBuilder.Entity<TodoList>()
            .HasMany(t => t.TodoItems)
            .WithOne()
            .HasForeignKey(ti => ti.TodoListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
