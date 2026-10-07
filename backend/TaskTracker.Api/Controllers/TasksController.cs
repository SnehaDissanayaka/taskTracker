using Microsoft.AspNetCore.Mvc;
using TaskTracker.Core.Dtos;
using TaskTracker.Core.Interfaces;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _tasks;

    public TasksController(ITaskService tasks)
    {
        _tasks = tasks;
    }

    // GET: api/tasks
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetTasks()
    {
        return Ok(await _tasks.GetAllAsync());
    }

    // GET: api/tasks/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskResponse>> GetTask(int id)
    {
        var task = await _tasks.GetByIdAsync(id);
        if (task is null) return NotFound();
        return task;
    }

    // POST: api/tasks
    [HttpPost]
    public async Task<ActionResult<TaskResponse>> CreateTask(CreateTaskRequest request)
    {
        var created = await _tasks.CreateAsync(request);
        return CreatedAtAction(nameof(GetTask), new { id = created.Id }, created);
    }

    // PUT: api/tasks/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, UpdateTaskRequest request)
    {
        return await _tasks.UpdateAsync(id, request) ? NoContent() : NotFound();
    }

    // DELETE: api/tasks/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        return await _tasks.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
