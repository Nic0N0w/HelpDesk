using HelpDeskPro.Wpf.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace HelpDeskPro.Wpf.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http)
    {
        _http = http;
    }

    // Tickets
    public Task<List<TicketDto>?> GetTicketsAsync(
        string? status = null, string? priority = null, string? search = null)
    {
        var url = BuildQuery("api/tickets", status, priority, search);
        return _http.GetFromJsonAsync<List<TicketDto>>(url);
    }

    public Task<TicketDto?> GetTicketAsync(int id) =>
        _http.GetFromJsonAsync<TicketDto>($"api/tickets/{id}");

    public Task<List<TicketDto>?> GetUserTicketsAsync(int userId) =>
        _http.GetFromJsonAsync<List<TicketDto>>($"api/users/{userId}/tickets");

    public async Task<TicketDto?> CreateTicketAsync(string title, string description, int priority, int userId)
    {
        var resp = await _http.PostAsJsonAsync("api/tickets",
            new { title, description, priority, createdByUserId = userId });
        return resp.IsSuccessStatusCode
            ? await resp.Content.ReadFromJsonAsync<TicketDto>() : null;
    }

    public async Task<(TicketDto? ticket, string? error)> UpdateStatusAsync(int id, string newStatusString)
    {
        // 0=Open, 1=InProgress, 2=Closed
        int newStatusInt = newStatusString switch
        {
            "Open" => 0,
            "InProgress" => 1,
            "Closed" => 2,
            _ => 0
        };

        var resp = await _http.PatchAsJsonAsync($"api/tickets/{id}/status", new { NewStatus = newStatusInt });
        if (resp.IsSuccessStatusCode)
            return (await resp.Content.ReadFromJsonAsync<TicketDto>(), null);

        var errorBody = await resp.Content.ReadAsStringAsync();
        return (null, errorBody);
    }

    public async Task<TicketDto?> AssignTicketAsync(int id, int userId)
    {
        var resp = await _http.PatchAsJsonAsync($"api/tickets/{id}/assign", new { assignedToUserId = userId });
        return resp.IsSuccessStatusCode
            ? await resp.Content.ReadFromJsonAsync<TicketDto>() : null;
    }

    public async Task<bool> AddCommentAsync(int ticketId, string text, int authorId)
    {
        var resp = await _http.PostAsJsonAsync($"api/tickets/{ticketId}/comments",
            new { text, authorId });
        return resp.IsSuccessStatusCode;
    }

    // Users
    public Task<List<UserDto>?> GetUsersAsync() =>
        _http.GetFromJsonAsync<List<UserDto>>("api/users");

    // Helper
    private static string BuildQuery(string baseUrl, string? status, string? priority, string? search)
    {
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(status)) parts.Add($"status={Uri.EscapeDataString(status)}");
        if (!string.IsNullOrEmpty(priority)) parts.Add($"priority={Uri.EscapeDataString(priority)}");
        if (!string.IsNullOrEmpty(search)) parts.Add($"search={Uri.EscapeDataString(search)}");
        return parts.Count == 0 ? baseUrl : $"{baseUrl}?{string.Join("&", parts)}";
    }
}
