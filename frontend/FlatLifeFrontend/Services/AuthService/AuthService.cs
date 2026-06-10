using System;
using System.Net.Http.Headers;
using System.Text.Json;
using FlatLifeFrontend.Models.BillSplitterModels;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;


namespace FlatLifeFrontend.Services;

public class AuthService(HttpClient client, IJSRuntime jsRuntime, NavigationManager navigationManager)
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;
    private readonly HttpClient _client = client;
    private readonly NavigationManager _navigationManager = navigationManager;

    public async Task HandleUnauthorizedResponse()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        _navigationManager.NavigateTo("/");
    }

    public async Task<bool> HandleHttpResponse(HttpResponseMessage response)
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        if (string.IsNullOrEmpty(token) || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await HandleUnauthorizedResponse();
            return false;
        }

        return true;
    }

    public async Task InitializeAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        if (!string.IsNullOrEmpty(token))
        {
            SetAuthorizationHeader(token);
        }
    }

    public async Task<bool> IsTokenValidAsync()
    {

        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        if (!string.IsNullOrEmpty(token))
        {
            return true;
        }
        return false;
    }



    public async Task<List<BillSplitterResponseBody>> GettBills()
    {
        var bills = new List<BillSplitterResponseBody>();
        try
        {
            HttpResponseMessage response = await _client.GetAsync("http://localhost:5080/api/BillSplitter/bills");

            response.EnsureSuccessStatusCode();

            string jsonResponseTask = await response.Content.ReadAsStringAsync();

            bills = JsonSerializer.Deserialize<List<BillSplitterResponseBody>>(jsonResponseTask);

        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return bills;

    }

    public async Task<string> GetUserId(IJSRuntime jsRuntime)
    {
        var token = await jsRuntime.InvokeAsync<string>("localStorage.getItem", "Id");
        return token ?? string.Empty;
    }

    public void SetAuthorizationHeader(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public HttpClient GetHttpClient()
    {
        return _client;
    }


}
