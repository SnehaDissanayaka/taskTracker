using Microsoft.EntityFrameworkCore;
using TaskTracker.Core.Interfaces;
using TaskTracker.Core.Models;

namespace TaskTracker.Data.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(Guid clientId)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Where(t => t.ClientId == clientId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(Guid clientId, int id)
    {
        return await _context.Tasks.FirstOrDefaultAsync(t => t.ClientId == clientId && t.Id == id);
    }

    public async Task<int> CountAsync(Guid clientId)
    {
        return await _context.Tasks.CountAsync(t => t.ClientId == clientId);
    }

    public async Task<TaskItem> AddAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task UpdateAsync(TaskItem task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TaskItem task)
    {
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }
}
