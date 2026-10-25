
using SmartTaskPlatform.Models;

namespace SmartTaskPlatform.Services;

public class TaskService
{
    private readonly List<TaskItem> _tasks = new();
    private int _nextId = 1;

    public List<TaskItem> GetAll()
    {
        return _tasks;
    }

    public void Add(string title, string priority)
    {
        var task = new TaskItem
        {
            Id = _nextId++,
            Title = title.Trim(),
            Priority = priority
        };

        _tasks.Add(task);
    }

    public void Update(TaskItem task, string title, string priority)
    {
        task.Title = title.Trim();
        task.Priority = priority;
    }

    public void Delete(TaskItem task)
    {
        _tasks.Remove(task);
    }
}
