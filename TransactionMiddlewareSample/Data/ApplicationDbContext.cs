using Microsoft.EntityFrameworkCore;
using TransactionMiddlewareSample.Models;

namespace TransactionMiddlewareSample.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }

        public DbSet<TodoList> TodoLists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoList>().HasMany(tl => tl.Items).WithOne().HasForeignKey(ti => ti.TodoListId);

            base.OnModelCreating(modelBuilder);
        }
    }
}