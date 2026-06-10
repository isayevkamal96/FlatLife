using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FlatLifeFrontend.Services;

public class TodoService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public TodoService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<List<TodoItem>> GetTasksAsync()
    {
        var response = await _authService
            .GetHttpClient()
            .GetAsync("http://localhost:5080/api/todo");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TodoItem>>(content) ?? new List<TodoItem>();
        }
        return new List<TodoItem>();
    }

    public async Task<bool> AddTaskAsync(TodoItem newItem)
    {
        var requestContent = new StringContent(
            JsonSerializer.Serialize(newItem),
            System.Text.Encoding.UTF8,
            "application/json"
        );

        var response = await _authService
            .GetHttpClient()
            .PostAsync("http://localhost:5080/api/todo", requestContent);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        var response = await _authService
            .GetHttpClient()
            .DeleteAsync($"http://localhost:5080/api/todo/{taskId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateTaskAsync(TodoItem task)
    {
        if (!string.IsNullOrWhiteSpace(task.Task))
        {
            var requestContent = new StringContent(
                JsonSerializer.Serialize(task),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await _authService
                .GetHttpClient()
                .PutAsync($"http://localhost:5080/api/todo/{task.Id}", requestContent);

            return response.IsSuccessStatusCode;
        }
        else
        {
            System.Console.WriteLine("whatever");
        }
        return false;
    }
}
