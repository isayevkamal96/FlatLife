using System;
using System.Net.Http.Headers;
using System.Text.Json;
using FlatLifeFrontend.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;



namespace FlatLifeFrontend.Services.HouseholdTaskService;

public class HouseholdTaskService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly NavigationManager _navigationManager;
    private readonly AuthService _authService;

    public HouseholdTaskService(IJSRuntime jsRuntime, NavigationManager navigationManager, AuthService authService)
    {
        _jsRuntime = jsRuntime;
        _navigationManager = navigationManager;
        _authService = authService;
    }
    public async Task<List<HouseholdTask>> GetTaskListAsync()
    {
        var tasks = new List<HouseholdTask>();

        await _authService.InitializeAsync();

        try
        {
            HttpResponseMessage response = await _authService.GetHttpClient().GetAsync("http://localhost:5080/api/FlatTask");

            bool isAuthorized = await _authService.HandleHttpResponse(response);

            if (!isAuthorized)
            {
                return tasks;
            }

            response.EnsureSuccessStatusCode();

            string jsonResponseTask = await response.Content.ReadAsStringAsync();

            tasks = JsonSerializer.Deserialize<List<HouseholdTask>>(jsonResponseTask);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return tasks;
    }

    public async Task<(bool TaskSuccess, string ErrorMessage)> AddTask(string taskNameInput, string taskFrequencyInput)
    {

        string formattedFrequency = "";
        
        if (!String.IsNullOrEmpty(taskFrequencyInput))
        {
            formattedFrequency = $"{taskFrequencyInput}:00:00:00";
        }

        var promptTask = new
        {
            TaskName = taskNameInput,
            Frequency = formattedFrequency
        };

        try
        {
            var promptContent = new StringContent(JsonSerializer.Serialize(promptTask), System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _authService.GetHttpClient().PostAsync("http://localhost:5080/api/FlatTask", promptContent);

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return (false, errorMessage);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return (false, ex.Message);
        }

    }
    public async Task<(bool TaskSuccess, string ErrorMessage)> EditTask(string taskNameInput, string taskFrequencyInput, int id)
    {

        string formattedFrequency = "";

        if (!String.IsNullOrEmpty(taskFrequencyInput))
        {
            formattedFrequency = $"{taskFrequencyInput}:00:00:00";
        }

        var promptTask = new
        {
            TaskName = taskNameInput,
            Frequency = formattedFrequency
        };
        try
        {
            var promptContent = new StringContent(JsonSerializer.Serialize(promptTask), System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _authService.GetHttpClient().PutAsync($"http://localhost:5080/api/FlatTask/{id}", promptContent);

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return (false, errorMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return (false, ex.Message);
        }
    }

    public async Task<bool> DeleteTask(int id)
    {
        try
        {
            HttpResponseMessage response = await _authService.GetHttpClient().DeleteAsync($"http://localhost:5080/api/FlatTask/{id}");
            return response.IsSuccessStatusCode;

        }
        catch (Exception e)
        {
            Console.WriteLine($"Unexpected error: {e.Message}");
        }
        return false;
    }

}
