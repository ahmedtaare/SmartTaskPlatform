
using Microsoft.EntityFrameworkCore;
using SmartTaskPlatform.Api.Models;

namespace SmartTaskPlatform.Api.Data;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();
}
