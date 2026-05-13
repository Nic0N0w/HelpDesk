using HelpDeskPro.Wpf.Models;
using HelpDeskPro.Wpf.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HelpDeskPro.Wpf.Views;

public partial class TicketListPage : Page
{
    private readonly ApiService _api;
    private readonly AuthState _auth;
    private readonly MainWindow _main;
    private List<TicketDto> _allTickets = new();

    public TicketListPage(ApiService api, AuthState auth, MainWindow main)
    {
        _api = api; _auth = auth; _main = main;
        InitializeComponent();
        Loaded += async (_, _) => await LoadTickets();
    }

    private async Task LoadTickets()
    {
        try
        {
            var status = (StatusFilter?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            var priority = (PriorityFilter?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            var search = SearchBox?.Text ?? "";

            if (status == "Alle Status") status = null;
            if (priority == "Alle Prioritäten") priority = null;
            if (string.IsNullOrWhiteSpace(search)) search = null;

            _allTickets = await _api.GetTicketsAsync(status, priority, search) ?? new();
            TicketGrid.ItemsSource = _allTickets;
            CountText.Text = $"{_allTickets.Count} Ticket(s) gefunden";
        }
        catch (Exception ex)
        {
            CountText.Text = $"Fehler beim Laden: {ex.Message}";
        }
    }

    private async void Filter_Changed(object sender, RoutedEventArgs e) => await LoadTickets();
    private async void Filter_Changed(object sender, SelectionChangedEventArgs e) => await LoadTickets();

    private async void ResetFilter_Click(object sender, RoutedEventArgs e)
    {
        SearchBox.Text = "";
        StatusFilter.SelectedIndex = 0;
        PriorityFilter.SelectedIndex = 0;
        await LoadTickets();
    }

    private void TicketGrid_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (TicketGrid.SelectedItem is TicketDto ticket)
            _main.NavigateToPage(new TicketDetailPage(_api, _auth, _main, ticket.Id), null);
    }

    private void NewTicket_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new NewTicketDialog(_api, _auth) { Owner = _main };
        if (dlg.ShowDialog() == true)
            _ = LoadTickets();
    }

    private void TicketGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}
