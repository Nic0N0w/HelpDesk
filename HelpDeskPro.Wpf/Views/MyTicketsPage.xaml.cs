using HelpDeskPro.Wpf.Models;
using HelpDeskPro.Wpf.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HelpDeskPro.Wpf.Views;

public partial class MyTicketsPage : Page
{
    private readonly ApiService _api;
    private readonly AuthState _auth;
    private readonly MainWindow _main;

    public MyTicketsPage(ApiService api, AuthState auth, MainWindow main)
    {
        _api = api; _auth = auth; _main = main;
        InitializeComponent();
        Loaded += async (_, _) => await Load();
    }

    private async Task Load()
    {
        HeaderText.Text = $"👤 Meine Tickets – {_auth.CurrentUser?.Name}";
        var tickets = await _api.GetUserTicketsAsync(_auth.CurrentUser!.Id) ?? new();
        MyGrid.ItemsSource = tickets;
        CountText.Text = $"{tickets.Count} Ticket(s)";
    }

    private void Grid_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (MyGrid.SelectedItem is TicketDto ticket)
            _main.NavigateToPage(new TicketDetailPage(_api, _auth, _main, ticket.Id), null);
    }
}
