using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options) { }

    public DbSet<TodoList> TodoList { get; set; } = default!;
    public DbSet<TodoItem> TodoItems { get; set; } = default!;

    // Add onModelcreating
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoList>()
            .HasMany(t => t.TodoItems)
            .WithOne()
            .HasForeignKey(ti => ti.TodoListId);

        // modelBuilder.Entity<TodoItem>()
        //     .Property(t => t.Description)
        //     .IsRequired()
        //     .OnDelete(DeleteBehavior.Cascade);
    }
}
