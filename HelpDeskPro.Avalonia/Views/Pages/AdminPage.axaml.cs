using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using HelpDeskPro.Avalonia.Models;
using HelpDeskPro.Avalonia.Services;

namespace HelpDeskPro.Avalonia.Views.Pages;

public partial class AdminPage : UserControl
{
    private readonly ApiService _api;
    private readonly AuthState _auth;
    private readonly MainWindow _main;

    public AdminPage() : this(App.ApiService, App.AuthState, null!) { }

    public AdminPage(ApiService api, AuthState auth, MainWindow main)
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
            var tickets = await _api.GetTicketsAsync() ?? new();

            TotalCount.Text = tickets.Count.ToString();
            OpenCount.Text = tickets.Count(t => t.Status == TicketStatus.Open).ToString();
            InProgressCount.Text = tickets.Count(t => t.Status == TicketStatus.InProgress).ToString();
            ClosedCount.Text = tickets.Count(t => t.Status == TicketStatus.Closed).ToString();

            AdminGrid.ItemsSource = tickets.OrderByDescending(t => t.Id).ToList();
        }
        catch (Exception ex)
        {
            TotalCount.Text = "!";
        }
    }

    private void Grid_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (AdminGrid.SelectedItem is TicketDto ticket)
            _main.NavigateToPage(new TicketDetailPage(_api, _auth, _main, ticket.Id), null);
    }
}
