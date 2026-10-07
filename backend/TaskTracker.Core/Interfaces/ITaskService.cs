using TaskTracker.Core.Dtos;

namespace TaskTracker.Core.Interfaces;

public interface ITaskService
{
    Task<IReadOnlyList<TaskResponse>> GetAllAsync();
    Task<TaskResponse?> GetByIdAsync(int id);
    Task<TaskResponse> CreateAsync(CreateTaskRequest request);
    Task<bool> UpdateAsync(int id, UpdateTaskRequest request);
    Task<bool> DeleteAsync(int id);
}
