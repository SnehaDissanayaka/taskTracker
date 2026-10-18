using Microsoft.EntityFrameworkCore;
using TaskTracker.Core.Models;

namespace TaskTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>().HasIndex(t => t.ClientId);
    }
}
