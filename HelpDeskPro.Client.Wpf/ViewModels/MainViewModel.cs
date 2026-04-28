using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HelpDeskPro.Client.Wpf.Models;
using HelpDeskPro.Client.Wpf.Services;
using System.Collections.ObjectModel;

namespace HelpDeskPro.Client.Wpf.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ApiService _api;

    public ObservableCollection<TicketResponseDto> Tickets { get; } = new();

    public MainViewModel(ApiService api)
    {
        _api = api;
        LoadCommand = new AsyncRelayCommand(LoadAsync);
        RefreshCommand = new RelayCommand(async () => await LoadAsync());
    }

    public IAsyncRelayCommand LoadCommand { get; }
    public IRelayCommand RefreshCommand { get; }

    private async Task LoadAsync()
    {
        var list = await _api.GetAllTicketsAsync();
        Tickets.Clear();
        if (list is null) return;
        foreach (var t in list) Tickets.Add(t);
    }
}
