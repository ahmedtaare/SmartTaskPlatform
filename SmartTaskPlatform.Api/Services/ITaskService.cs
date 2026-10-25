
using SmartTaskPlatform.Api.Models;

namespace SmartTaskPlatform.Api.Services;

public interface ITaskService
{
    Task<List<TaskItem>> GetAllAsync();

    Task<TaskItem?> GetByIdAsync(int id);

    Task<TaskItem> CreateAsync(
        string title,
        string priority);

    Task<TaskItem?> UpdateAsync(
        int id,
        string title,
        string priority,
        bool isCompleted);

    Task<bool> DeleteAsync(int id);
}
