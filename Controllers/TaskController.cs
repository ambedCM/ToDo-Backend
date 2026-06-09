using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Todo.Data;
using Todo.DTO;
using Todo.DTOs;
using Todo.Models;
using Todo.Hubs;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<TaskHub> _hubContext;

    public TaskController(ApplicationDbContext context, IHubContext<TaskHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    [HttpGet]
    public IActionResult GetTasks()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var tasks = _context.Tasks
            .Where(t => t.UserId == userId)
            .ToList();

        return Ok(tasks);
    }

    [HttpPost]
    public IActionResult CreateTask(CreateTask dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var task = new TodoTask
        {
            Title = dto.Title,
            DueDate = dto.DueDate,
            Status = "Pending",
            UserId = userId
        };

        _context.Tasks.Add(task);
        _context.SaveChanges();

        _hubContext.Clients.All.SendAsync("TaskAdded", task);

        return Ok(new
        {
            message = "Task added successfully",
            task
        });
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTask(int id, CreateTask dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var task = _context.Tasks
            .FirstOrDefault(t => t.Id == id && t.UserId == userId);

        if (task == null)
        {
            return NotFound(new { message = "Task not found" });
        }

        task.Title = dto.Title;
        task.DueDate = dto.DueDate;

        _context.SaveChanges();

        _hubContext.Clients.All.SendAsync("TaskUpdated", task);

        return Ok(new
        {
            message = "Task updated successfully",
            task
        });
    }

    [HttpPatch("{id}/complete")]
    public IActionResult CompleteTask(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var task = _context.Tasks
            .FirstOrDefault(t => t.Id == id && t.UserId == userId);

        if (task == null)
        {
            return NotFound(new { message = "Task not found" });
        }

        task.Status = "Completed";

        _context.SaveChanges();

        _hubContext.Clients.All.SendAsync("TaskUpdated", task);

        return Ok(new
        {
            message = "Task completed successfully",
            task
        });
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTask(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var task = _context.Tasks
            .FirstOrDefault(t => t.Id == id && t.UserId == userId);

        if (task == null)
        {
            return NotFound(new { message = "Task not found" });
        }

        _context.Tasks.Remove(task);
        _context.SaveChanges();

        _hubContext.Clients.All.SendAsync("TaskDeleted", id);

        return Ok(new { message = "Task deleted successfully" });
    }
}