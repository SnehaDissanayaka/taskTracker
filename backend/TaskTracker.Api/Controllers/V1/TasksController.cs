using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Core.Dtos;
using TaskTracker.Core.Interfaces;

namespace TaskTracker.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _tasks;

    public TasksController(ITaskService tasks)
    {
        _tasks = tasks;
    }

    // GET: api/v1/tasks
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetTasks()
    {
        return Ok(await _tasks.GetAllAsync());
    }

    // GET: api/v1/tasks/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskResponse>> GetTask(int id)
    {
        var task = await _tasks.GetByIdAsync(id);
        if (task is null) return NotFound();
        return task;
    }

    // POST: api/v1/tasks
    [HttpPost]
    public async Task<ActionResult<TaskResponse>> CreateTask(CreateTaskRequest request)
    {
        var created = await _tasks.CreateAsync(request);
        return CreatedAtAction(nameof(GetTask), new { id = created.Id }, created);
    }

    // PUT: api/v1/tasks/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, UpdateTaskRequest request)
    {
        return await _tasks.UpdateAsync(id, request) ? NoContent() : NotFound();
    }

    // DELETE: api/v1/tasks/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        return await _tasks.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
