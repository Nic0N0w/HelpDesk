using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using HelpDeskPro.Avalonia.Models;
using HelpDeskPro.Avalonia.Services;

namespace HelpDeskPro.Avalonia.Views.Pages;

public partial class MyTicketsPage : UserControl
{
    private readonly ApiService _api;
    private readonly AuthState _auth;
    private readonly MainWindow _main;

    public MyTicketsPage() : this(App.ApiService, App.AuthState, null!) { }

    public MyTicketsPage(ApiService api, AuthState auth, MainWindow main)
    {
        _api = api;
        _auth = auth;
        _main = main;
        InitializeComponent();
        Loaded += async (_, _) => await Load();
    }

    private async Task Load()
    {
        try
        {
            HeaderText.Text = $"👤 Meine Tickets – {_auth.CurrentUser?.Name}";
            var tickets = await _api.GetUserTicketsAsync(_auth.CurrentUser!.Id) ?? new();
            MyGrid.ItemsSource = tickets;
            CountText.Text = $"{tickets.Count} Ticket(s)";
        }
        catch (Exception ex)
        {
            CountText.Text = $"Fehler: {ex.Message}";
        }
    }

    private void Grid_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (MyGrid.SelectedItem is TicketDto ticket)
            _main.NavigateToPage(new TicketDetailPage(_api, _auth, _main, ticket.Id), null);
    }
}
