using TaskTracker.Core.Configuration;
using TaskTracker.Core.Dtos;
using TaskTracker.Core.Exceptions;
using TaskTracker.Core.Interfaces;
using TaskTracker.Core.Models;

namespace TaskTracker.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly TaskLimits _limits;

    public TaskService(ITaskRepository repository, TaskLimits limits)
    {
        _repository = repository;
        _limits = limits;
    }

    public async Task<IReadOnlyList<TaskResponse>> GetAllAsync(Guid clientId)
    {
        var tasks = await _repository.GetAllAsync(clientId);
        return tasks.Select(ToResponse).ToList();
    }

    public async Task<TaskResponse?> GetByIdAsync(Guid clientId, int id)
    {
        var task = await _repository.GetByIdAsync(clientId, id);
        return task is null ? null : ToResponse(task);
    }

    public async Task<TaskResponse> CreateAsync(Guid clientId, CreateTaskRequest request)
    {
        if (await _repository.CountAsync(clientId) >= _limits.MaxTasksPerClient)
        {
            throw new TaskLimitExceededException(_limits.MaxTasksPerClient);
        }

        var task = new TaskItem
        {
            Title = request.Title.Trim(),
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            ClientId = clientId,
        };
        var created = await _repository.AddAsync(task);
        return ToResponse(created);
    }

    public async Task<bool> UpdateAsync(Guid clientId, int id, UpdateTaskRequest request)
    {
        var task = await _repository.GetByIdAsync(clientId, id);
        if (task is null) return false;

        task.Title = request.Title.Trim();
        task.Description = request.Description;
        task.IsComplete = request.IsComplete;

        await _repository.UpdateAsync(task);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid clientId, int id)
    {
        var task = await _repository.GetByIdAsync(clientId, id);
        if (task is null) return false;

        await _repository.DeleteAsync(task);
        return true;
    }

    private static TaskResponse ToResponse(TaskItem t) =>
        new(t.Id, t.Title, t.Description, t.IsComplete, t.CreatedAt);
}
