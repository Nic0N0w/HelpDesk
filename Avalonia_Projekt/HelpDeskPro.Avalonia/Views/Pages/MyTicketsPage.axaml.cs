using Avalonia.Controls;
using Avalonia.Input;
using HelpDeskPro.Avalonia.Models;
using System.Linq;
using System.Threading.Tasks;

namespace HelpDeskPro.Avalonia.Views.Pages;

public partial class MyTicketsPage : UserControl
{
    private readonly MainWindow _main;

    public MyTicketsPage(MainWindow main)
    {
        _main = main;
        InitializeComponent();
        Loaded += async (_, _) => await Load();
    }

    private async Task Load()
    {
        HeaderText.Text = $"👤 Meine Tickets – {App.AuthState.CurrentUser?.Name}";
        var userId = App.AuthState.CurrentUser!.Id;

        // Tickets die der User erstellt hat
        var createdTickets = await App.ApiService.GetUserTicketsAsync(userId) ?? new();

        // Tickets die dem User zugewiesen wurden
        var allTickets = await App.ApiService.GetTicketsAsync() ?? new();
        var assignedTickets = allTickets.Where(t => t.AssignedToUserId == userId).ToList();

        // Kombinieren und Duplikate entfernen
        var combined = createdTickets.Union(assignedTickets, new TicketDtoComparer()).OrderByDescending(t => t.Id).ToList();

        MyGrid.ItemsSource = combined;
        CountText.Text = $"{combined.Count} Ticket(s)";
    }

    private class TicketDtoComparer : IEqualityComparer<TicketDto>
    {
        public bool Equals(TicketDto? x, TicketDto? y) => x?.Id == y?.Id;
        public int GetHashCode(TicketDto obj) => obj.Id.GetHashCode();
    }

    private void Grid_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (MyGrid.SelectedItem is TicketDto ticket)
            _main.NavigateToPage(new TicketDetailPage(_main, ticket.Id), null);
    }
}
