using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Core.Dtos;

public record UpdateTaskRequest
{
    [Required, MaxLength(200)]
    public string Title { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; init; }

    public bool IsComplete { get; init; }
}
