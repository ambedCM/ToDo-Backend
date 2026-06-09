using Todo.Data;
using Todo.DTO;
using Todo.DTOs;
using Todo.Models;

namespace Todo.Services;

public class TaskService
{
    private readonly ApplicationDbContext _context;

    public TaskService(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<TodoTask> GetTasks(int userId)
    {
        return _context.Tasks
            .Where(t => t.UserId == userId)
            .ToList();
    }

    public TodoTask CreateTask(CreateTask dto, int userId)
    {
        var task = new TodoTask
        {
            Title = dto.Title,
            DueDate = dto.DueDate,
            Status = "Pending",
            UserId = userId
        };

        _context.Tasks.Add(task);
        _context.SaveChanges();

        return task;
    }

    public TodoTask? UpdateTask(int id, CreateTask dto, int userId)
    {
        var task = _context.Tasks
            .FirstOrDefault(t => t.Id == id && t.UserId == userId);

        if (task == null)
            return null;

        task.Title = dto.Title;
        task.DueDate = dto.DueDate;

        _context.SaveChanges();

        return task;
    }

    public TodoTask? CompleteTask(int id, int userId)
    {
        var task = _context.Tasks
            .FirstOrDefault(t => t.Id == id && t.UserId == userId);

        if (task == null)
            return null;

        task.Status = "Completed";

        _context.SaveChanges();

        return task;
    }

    public bool DeleteTask(int id, int userId)
    {
        var task = _context.Tasks
            .FirstOrDefault(t => t.Id == id && t.UserId == userId);

        if (task == null)
            return false;

        _context.Tasks.Remove(task);
        _context.SaveChanges();

        return true;
    }
}