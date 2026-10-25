
namespace SmartTaskPlatform.Api.Models;

public class TaskItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Priority { get; set; } = "Medium";

    public bool IsCompleted { get; set; }
}
