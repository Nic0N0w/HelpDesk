using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using HelpDeskPro.Avalonia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpDeskPro.Avalonia.Views.Pages;

public partial class TicketListPage : UserControl
{
    private readonly MainWindow _main;
    private List<TicketDto> _allTickets = new();
    private bool _loading;

    public TicketListPage(MainWindow main)
    {
        _main = main;
        InitializeComponent();
        Loaded += async (_, _) => await LoadTickets();
        SearchBox.TextChanged += async (_, _) => await LoadTickets();
    }

    private async Task LoadTickets()
    {
        if (_loading) return;
        _loading = true;
        try
        {
            var status = (StatusFilter?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            var priority = (PriorityFilter?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            var search = SearchBox?.Text ?? "";

            if (status == "Alle Status") status = null;
            if (priority == "Alle Prioritäten") priority = null;
            if (string.IsNullOrWhiteSpace(search)) search = null;

            _allTickets = await App.ApiService.GetTicketsAsync(status, priority, search) ?? new();
            TicketGrid.ItemsSource = _allTickets;
            CountText.Text = $"{_allTickets.Count} Ticket(s) gefunden";
        }
        catch (Exception ex)
        {
            CountText.Text = $"Fehler beim Laden: {ex.Message}";
        }
        finally
        {
            _loading = false;
        }
    }

    private async void Filter_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        => await LoadTickets();

    private async void ResetFilter_Click(object sender, RoutedEventArgs e)
    {
        SearchBox.Text = string.Empty;
        StatusFilter.SelectedIndex = 0;
        PriorityFilter.SelectedIndex = 0;
        await LoadTickets();
    }

    private void TicketGrid_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (TicketGrid.SelectedItem is TicketDto ticket)
            _main.NavigateToPage(new TicketDetailPage(_main, ticket.Id), null);
    }

    private void TicketGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e) { }

    private void NewTicket_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new NewTicketDialog();
        dialog.Closed += async (_, _) =>
        {
            if (dialog.Success)
                await LoadTickets();
        };
        dialog.ShowDialog(TopLevel.GetTopLevel(this) as Window ?? _main);
    }
}
