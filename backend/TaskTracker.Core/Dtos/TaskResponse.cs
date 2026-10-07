namespace TaskTracker.Core.Dtos;

public record TaskResponse(
    int Id,
    string Title,
    string? Description,
    bool IsComplete,
    DateTime CreatedAt);
