
using Microsoft.EntityFrameworkCore;
using SmartTaskPlatform.Api.Data;
using SmartTaskPlatform.Api.Models;

namespace SmartTaskPlatform.Api.Services;

public class TaskService : ITaskService
{
    private readonly TaskDbContext _db;

    public TaskService(TaskDbContext db)
    {
        _db = db;
    }

    public async Task<List<TaskItem>> GetAllAsync()
    {
        return await _db.Tasks
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _db.Tasks.FindAsync(id);
    }

    public async Task<TaskItem> CreateAsync(
        string title,
        string priority)
    {
        var task = new TaskItem
        {
            Title = title.Trim(),
            Priority = priority,
            IsCompleted = false
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        return task;
    }

    public async Task<TaskItem?> UpdateAsync(
        int id,
        string title,
        string priority,
        bool isCompleted)
    {
        var task = await _db.Tasks.FindAsync(id);

        if (task is null)
        {
            return null;
        }

        task.Title = title.Trim();
        task.Priority = priority;
        task.IsCompleted = isCompleted;

        await _db.SaveChangesAsync();

        return task;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _db.Tasks.FindAsync(id);

        if (task is null)
        {
            return false;
        }

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();

        return true;
    }
}
