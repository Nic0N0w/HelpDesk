using System.Net.Http.Json;
using HelpDeskPro.Client.Wpf.Models;

namespace HelpDeskPro.Client.Wpf.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http)
    {
        _http = http;
    }

    public Task<List<TicketResponseDto>?> GetAllTicketsAsync() =>
        _http.GetFromJsonAsync<List<TicketResponseDto>>("api/tickets");

    public async Task<TicketResponseDto?> GetTicketByIdAsync(int id) =>
        await _http.GetFromJsonAsync<TicketResponseDto>($"api/tickets/{id}");

    public async Task<bool> CreateTicketAsync(object createRequest)
    {
        var res = await _http.PostAsJsonAsync("api/tickets", createRequest);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateStatusAsync(int id, object updateStatusRequest)
    {
        var res = await _http.PatchAsJsonAsync($"api/tickets/{id}/status", updateStatusRequest);
        return res.IsSuccessStatusCode;
    }
}
