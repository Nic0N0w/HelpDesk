using HelpDeskPro.Wpf.Models;
using HelpDeskPro.Wpf.Services;
using System.Windows.Controls;
using System.Windows.Input;

namespace HelpDeskPro.Wpf.Views;

public partial class AdminPage : Page
{
    private readonly ApiService _api;
    private readonly AuthState _auth;
    private readonly MainWindow _main;

    public AdminPage(ApiService api, AuthState auth, MainWindow main)
    {
        _api = api; _auth = auth; _main = main;
        InitializeComponent();
        Loaded += async (_, _) => await Load();
    }

    private async Task Load()
    {
        var tickets = await _api.GetTicketsAsync() ?? new();

        TotalCount.Text = tickets.Count.ToString();
        OpenCount.Text = tickets.Count(t => t.Status == TicketStatus.Open).ToString();
        InProgressCount.Text = tickets.Count(t => t.Status == TicketStatus.InProgress).ToString();
        ClosedCount.Text = tickets.Count(t => t.Status == TicketStatus.Closed).ToString();

        AdminGrid.ItemsSource = tickets.OrderByDescending(t => t.Id).ToList();
    }

    private void Grid_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (AdminGrid.SelectedItem is TicketDto ticket)
            _main.NavigateToPage(new TicketDetailPage(_api, _auth, _main, ticket.Id), null);
    }
}
