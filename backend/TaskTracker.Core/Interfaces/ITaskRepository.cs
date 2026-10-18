using TaskTracker.Core.Models;

namespace TaskTracker.Core.Interfaces;

public interface ITaskRepository
{
    Task<IReadOnlyList<TaskItem>> GetAllAsync(Guid clientId);
    Task<TaskItem?> GetByIdAsync(Guid clientId, int id);
    Task<TaskItem> AddAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);
}
