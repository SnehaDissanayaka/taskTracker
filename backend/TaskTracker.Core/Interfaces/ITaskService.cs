using TaskTracker.Core.Dtos;

namespace TaskTracker.Core.Interfaces;

public interface ITaskService
{
    Task<IReadOnlyList<TaskResponse>> GetAllAsync(Guid clientId);
    Task<TaskResponse?> GetByIdAsync(Guid clientId, int id);
    Task<TaskResponse> CreateAsync(Guid clientId, CreateTaskRequest request);
    Task<bool> UpdateAsync(Guid clientId, int id, UpdateTaskRequest request);
    Task<bool> DeleteAsync(Guid clientId, int id);
}
