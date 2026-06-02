using Avalonia.Controls;
using Avalonia.Input;
using HelpDeskPro.Avalonia.Models;
using System.Linq;
using System.Threading.Tasks;

namespace HelpDeskPro.Avalonia.Views.Pages;

public partial class AdminPage : UserControl
{
    private readonly MainWindow _main;

    public AdminPage(MainWindow main)
    {
        _main = main;
        InitializeComponent();
        Loaded += async (_, _) => await Load();
    }

    private async Task Load()
    {
        var tickets = await App.ApiService.GetTicketsAsync() ?? new();

        TotalCount.Text = tickets.Count.ToString();
        OpenCount.Text = tickets.Count(t => t.Status == TicketStatus.Open).ToString();
        InProgressCount.Text = tickets.Count(t => t.Status == TicketStatus.InProgress).ToString();
        ClosedCount.Text = tickets.Count(t => t.Status == TicketStatus.Closed).ToString();

        AdminGrid.ItemsSource = tickets.OrderByDescending(t => t.Id).ToList();
    }

    private void Grid_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (AdminGrid.SelectedItem is TicketDto ticket)
            _main.NavigateToPage(new TicketDetailPage(_main, ticket.Id), null);
    }
}
