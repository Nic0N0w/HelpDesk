using HelpDeskPro.Frontend.Models;
using System.Net.Http.Json;

namespace HelpDeskPro.Frontend.Services;

// ── API-Service ─────────────────────────────────────────
public class ApiService(HttpClient http)
{
    public Task<List<TicketDto>?> GetTicketsAsync() =>
        http.GetFromJsonAsync<List<TicketDto>>("api/tickets");

    public Task<List<TicketDto>?> GetUserTicketsAsync(int userId) =>
        http.GetFromJsonAsync<List<TicketDto>>($"api/users/{userId}/tickets");

    public Task<List<UserDto>?> GetUsersAsync() =>
        http.GetFromJsonAsync<List<UserDto>>("api/users");

    public async Task<TicketDto?> CreateTicketAsync(object payload)
    {
        var resp = await http.PostAsJsonAsync("api/tickets", payload);
        return resp.IsSuccessStatusCode
            ? await resp.Content.ReadFromJsonAsync<TicketDto>()
            : null;
    }

    public async Task<TicketDto?> UpdateStatusAsync(int id, string newStatus)
    {
        var resp = await http.PatchAsJsonAsync($"api/tickets/{id}/status", new { NewStatus = newStatus });
        return resp.IsSuccessStatusCode
            ? await resp.Content.ReadFromJsonAsync<TicketDto>()
            : null;
    }
}

// ── Auth-State (Mock für M1) ────────────────────────────
public class AuthState
{
    public UserDto? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser is not null;

    public event Action? OnChange;

    public void Login(UserDto user)
    {
        CurrentUser = user;
        OnChange?.Invoke();
    }

    public void Logout()
    {
        CurrentUser = null;
        OnChange?.Invoke();
    }
}
