
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SmartTaskPlatform.Api.Tests;

public class TasksApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public TasksApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetTasks_ReturnsSuccessAndTaskList()
    {
        await _factory.InitializeDatabaseAsync();

        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tasks = await response.Content
            .ReadFromJsonAsync<List<TaskResponse>>();

        Assert.NotNull(tasks);
        Assert.Empty(tasks);
    }

    [Fact]
    public async Task CreateTask_WithValidData_ReturnsCreated()
    {
        await _factory.InitializeDatabaseAsync();

        using var client = _factory.CreateClient();

        var request = new
        {
            title = "Learn automated testing",
            priority = "High"
        };

        var response = await client.PostAsJsonAsync(
            "/api/tasks",
            request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var task = await response.Content
            .ReadFromJsonAsync<TaskResponse>();

        Assert.NotNull(task);
        Assert.True(task.Id > 0);
        Assert.Equal("Learn automated testing", task.Title);
        Assert.Equal("High", task.Priority);
        Assert.False(task.IsCompleted);
    }


    [Fact]
    public async Task UpdateTask_WithValidData_ReturnsUpdatedTask()
    {
        await _factory.InitializeDatabaseAsync();

        using var client = _factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/api/tasks",
            new
            {
                title = "Original task",
                priority = "Low"
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdTask = await createResponse.Content
            .ReadFromJsonAsync<TaskResponse>();

        Assert.NotNull(createdTask);

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/tasks/{createdTask.Id}",
            new
            {
                title = "Updated task",
                priority = "High",
                isCompleted = true
            });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedTask = await updateResponse.Content
            .ReadFromJsonAsync<TaskResponse>();

        Assert.NotNull(updatedTask);
        Assert.Equal("Updated task", updatedTask.Title);
        Assert.Equal("High", updatedTask.Priority);
        Assert.True(updatedTask.IsCompleted);
    }

    [Fact]
    public async Task DeleteTask_WithExistingId_ReturnsNoContent()
    {
        await _factory.InitializeDatabaseAsync();

        using var client = _factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/api/tasks",
            new
            {
                title = "Task to delete",
                priority = "Medium"
            });

        var createdTask = await createResponse.Content
            .ReadFromJsonAsync<TaskResponse>();

        Assert.NotNull(createdTask);

        var deleteResponse = await client.DeleteAsync(
            $"/api/tasks/{createdTask.Id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);

        var getResponse = await client.GetAsync(
            $"/api/tasks/{createdTask.Id}");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetTask_WithUnknownId_ReturnsNotFound()
    {
        await _factory.InitializeDatabaseAsync();

        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/tasks/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateTask_WithInvalidTitle_ReturnsBadRequest()
    {
        await _factory.InitializeDatabaseAsync();

        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/tasks",
            new
            {
                title = "A",
                priority = "High"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private sealed class TaskResponse
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }
    }
}
