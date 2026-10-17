namespace TaskTracker.Core.Exceptions;

public class TaskLimitExceededException : Exception
{
    public TaskLimitExceededException(int limit)
        : base($"A client can have at most {limit} tasks.")
    {
        Limit = limit;
    }

    public int Limit { get; }
}
