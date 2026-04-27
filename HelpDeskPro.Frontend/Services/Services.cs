using HelpDeskPro.Frontend.Models;
using System.Net.Http.Json;

namespace HelpDeskPro.Frontend.Services;

public class ApiService(HttpClient http)
{
    // ── Tickets ─────────────────────────────────────────
    public Task<List<TicketDto>?> GetTicketsAsync(
        string? status = null, string? priority = null, string? search = null) =>
        http.GetFromJsonAsync<List<TicketDto>>(BuildQuery("api/tickets", status, priority, search));

    public Task<List<TicketDto>?> GetUserTicketsAsync(int userId) =>
        http.GetFromJsonAsync<List<TicketDto>>($"api/users/{userId}/tickets");

    public Task<TicketDto?> GetTicketAsync(int id) =>
        http.GetFromJsonAsync<TicketDto>($"api/tickets/{id}");

    public async Task<TicketDto?> CreateTicketAsync(string title, string description, int priority, int userId)
    {
        var resp = await http.PostAsJsonAsync("api/tickets", new { title, description, priority, createdByUserId = userId });
        return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<TicketDto>() : null;
    }

    public async Task<(TicketDto? ticket, string? error)> UpdateStatusAsync(int id, string newStatus)
    {
        var resp = await http.PatchAsJsonAsync($"api/tickets/{id}/status", new { newStatus = ToStatusNumber(newStatus) });
        if (resp.IsSuccessStatusCode)
            return (await resp.Content.ReadFromJsonAsync<TicketDto>(), null);
        var err = await resp.Content.ReadAsStringAsync();
        return (null, err);
    }

    private static int ToStatusNumber(string status)
        => status switch
        {
            "Open" => 0,
            "InProgress" => 1,
            "Closed" => 2,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unbekannter TicketStatus")
        };

    public async Task<TicketDto?> AssignTicketAsync(int id, int assignedToUserId)
    {
        var resp = await http.PatchAsJsonAsync($"api/tickets/{id}/assign", new { assignedToUserId });
        return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<TicketDto>() : null;
    }

    public async Task<bool> AddCommentAsync(int ticketId, string text, int authorId)
    {
        var resp = await http.PostAsJsonAsync($"api/tickets/{ticketId}/comments", new { text, authorId });
        return resp.IsSuccessStatusCode;
    }

    // ── Users ────────────────────────────────────────────
    public Task<List<UserDto>?> GetUsersAsync() =>
        http.GetFromJsonAsync<List<UserDto>>("api/users");

    // ── Helpers ──────────────────────────────────────────
    private static string BuildQuery(string base_, string? status, string? priority, string? search)
    {
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(status)) parts.Add($"status={Uri.EscapeDataString(status)}");
        if (!string.IsNullOrEmpty(priority)) parts.Add($"priority={Uri.EscapeDataString(priority)}");
        if (!string.IsNullOrEmpty(search)) parts.Add($"search={Uri.EscapeDataString(search)}");
        return parts.Count == 0 ? base_ : $"{base_}?{string.Join("&", parts)}";
    }
}

public class AuthState
{
    public UserDto? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser is not null;
    public event Action? OnChange;

    public void Login(UserDto user) { CurrentUser = user; OnChange?.Invoke(); }
    public void Logout() { CurrentUser = null; OnChange?.Invoke(); }
}
