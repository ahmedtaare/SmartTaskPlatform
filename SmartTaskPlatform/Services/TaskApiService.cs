
using System.Net.Http.Json;
using SmartTaskPlatform.Models;

namespace SmartTaskPlatform.Services;

public class TaskApiService
{
    private readonly HttpClient _httpClient;

    public TaskApiService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("SmartTaskApi");
    }

    public async Task DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/tasks/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<TaskItem>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<TaskItem>>(
            "api/tasks") ?? new List<TaskItem>();
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<TaskItem>(
            $"api/tasks/{id}");
    }

    public async Task<TaskItem?> CreateAsync(
        string title,
        string priority)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/tasks",
            new { title, priority });

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TaskItem>();
    }

    public async Task<TaskItem?> UpdateAsync(
        int id,
        string title,
        string priority,
        bool isCompleted)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/tasks/{id}",
            new
            {
                title,
                priority,
                isCompleted
            });

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TaskItem>();
    }

}
