
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SmartTaskPlatform.Api.Data;
using SmartTaskPlatform.Api.DTOs;
using SmartTaskPlatform.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Register OpenAPI
builder.Services.AddOpenApi();

// 2. Register SQLite database
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlite("Data Source=smarttask.db"));

// 3. Register Task Service
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddProblemDetails();
var app = builder.Build();
app.UseExceptionHandler();

// 4. Configure HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// 5. GET: Get all tasks
app.MapGet("/api/tasks", async (ITaskService service) =>
{
    var tasks = await service.GetAllAsync();
    return Results.Ok(tasks);
});

// 6. GET: Get task by ID
app.MapGet("/api/tasks/{id:int}", async (
    int id,
    ITaskService service) =>
{
    var task = await service.GetByIdAsync(id);

    return task is null
        ? Results.NotFound(new
        {
            message = $"Task with ID {id} was not found."
        })
        : Results.Ok(task);
});

// 7. POST: Create task
app.MapPost("/api/tasks", async (
    CreateTaskRequest request,
    ITaskService service) =>
{
    var errors = ValidateRequest(request);

    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var task = await service.CreateAsync(
        request.Title,
        request.Priority);

    return Results.Created(
        $"/api/tasks/{task.Id}",
        task);
});

// 8. PUT: Update task
app.MapPut("/api/tasks/{id:int}", async (
    int id,
    UpdateTaskRequest request,
    ITaskService service) =>
{
    var errors = ValidateRequest(request);

    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var task = await service.UpdateAsync(
        id,
        request.Title,
        request.Priority,
        request.IsCompleted);

    return task is null
        ? Results.NotFound(new
        {
            message = $"Task with ID {id} was not found."
        })
        : Results.Ok(task);
});

// 9. DELETE: Delete task
app.MapDelete("/api/tasks/{id:int}", async (
    int id,
    ITaskService service) =>
{
    var deleted = await service.DeleteAsync(id);

    return deleted
        ? Results.NoContent()
        : Results.NotFound(new
        {
            message = $"Task with ID {id} was not found."
        });
});

app.Run();

// 10. Validate DTOs using Data Annotations
static Dictionary<string, string[]> ValidateRequest<T>(T request)
{
    var validationResults = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(
        request!,
        new ValidationContext(request!),
        validationResults,
        validateAllProperties: true);

    if (isValid)
    {
        return new Dictionary<string, string[]>();
    }

    return validationResults
        .SelectMany(result =>
            (result.MemberNames.Any()
                ? result.MemberNames
                : new[] { string.Empty })
            .Select(member => new
            {
                Member = member,
                Error = result.ErrorMessage ?? "Invalid value."
            }))
        .GroupBy(item => item.Member)
        .ToDictionary(
            group => group.Key,
            group => group
                .Select(item => item.Error)
                .ToArray());
}
public partial class Program { }
