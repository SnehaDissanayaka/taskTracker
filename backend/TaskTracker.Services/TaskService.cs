using TaskTracker.Core.Dtos;
using TaskTracker.Core.Interfaces;
using TaskTracker.Core.Models;

namespace TaskTracker.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TaskResponse>> GetAllAsync()
    {
        var tasks = await _repository.GetAllAsync();
        return tasks.Select(ToResponse).ToList();
    }

    public async Task<TaskResponse?> GetByIdAsync(int id)
    {
        var task = await _repository.GetByIdAsync(id);
        return task is null ? null : ToResponse(task);
    }

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request)
    {
        var task = new TaskItem
        {
            Title = request.Title.Trim(),
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
        };
        var created = await _repository.AddAsync(task);
        return ToResponse(created);
    }

    public async Task<bool> UpdateAsync(int id, UpdateTaskRequest request)
    {
        var task = await _repository.GetByIdAsync(id);
        if (task is null) return false;

        task.Title = request.Title.Trim();
        task.Description = request.Description;
        task.IsComplete = request.IsComplete;

        await _repository.UpdateAsync(task);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _repository.GetByIdAsync(id);
        if (task is null) return false;

        await _repository.DeleteAsync(task);
        return true;
    }

    private static TaskResponse ToResponse(TaskItem t) =>
        new(t.Id, t.Title, t.Description, t.IsComplete, t.CreatedAt);
}
