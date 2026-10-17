using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Core.Dtos;
using TaskTracker.Core.Exceptions;
using TaskTracker.Core.Interfaces;

namespace TaskTracker.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class TasksController : ControllerBase
{
    private const string ClientIdHeader = "X-Client-Id";

    private readonly ITaskService _tasks;

    public TasksController(ITaskService tasks)
    {
        _tasks = tasks;
    }

    // Every request carries an anonymous, browser-generated ID (X-Client-Id) that scopes the caller's tasks.
    // A missing or malformed header is rejected by model binding; the all-zero GUID is reserved for pre-existing rows.
    private ActionResult? RejectEmptyClientId(Guid clientId) =>
        clientId == Guid.Empty
            ? Problem(statusCode: StatusCodes.Status400BadRequest, title: $"A non-empty {ClientIdHeader} header is required.")
            : null;

    // GET: api/v1/tasks
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetTasks([FromHeader(Name = ClientIdHeader)] Guid clientId)
    {
        if (RejectEmptyClientId(clientId) is { } rejected) return rejected;
        return Ok(await _tasks.GetAllAsync(clientId));
    }

    // GET: api/v1/tasks/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskResponse>> GetTask([FromHeader(Name = ClientIdHeader)] Guid clientId, int id)
    {
        if (RejectEmptyClientId(clientId) is { } rejected) return rejected;
        var task = await _tasks.GetByIdAsync(clientId, id);
        if (task is null) return NotFound();
        return task;
    }

    // POST: api/v1/tasks
    [HttpPost]
    public async Task<ActionResult<TaskResponse>> CreateTask([FromHeader(Name = ClientIdHeader)] Guid clientId, CreateTaskRequest request)
    {
        if (RejectEmptyClientId(clientId) is { } rejected) return rejected;
        try
        {
            var created = await _tasks.CreateAsync(clientId, request);
            return CreatedAtAction(nameof(GetTask), new { id = created.Id }, created);
        }
        catch (TaskLimitExceededException ex)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Task limit reached", detail: ex.Message);
        }
    }

    // PUT: api/v1/tasks/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask([FromHeader(Name = ClientIdHeader)] Guid clientId, int id, UpdateTaskRequest request)
    {
        if (RejectEmptyClientId(clientId) is { } rejected) return rejected;
        return await _tasks.UpdateAsync(clientId, id, request) ? NoContent() : NotFound();
    }

    // DELETE: api/v1/tasks/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask([FromHeader(Name = ClientIdHeader)] Guid clientId, int id)
    {
        if (RejectEmptyClientId(clientId) is { } rejected) return rejected;
        return await _tasks.DeleteAsync(clientId, id) ? NoContent() : NotFound();
    }
}
