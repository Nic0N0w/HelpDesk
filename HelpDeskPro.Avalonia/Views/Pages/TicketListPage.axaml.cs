using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace HelpDeskPro.Avalonia.Views.Pages;

public partial class TicketListPage : UserControl
{
    public TicketListPage()
    {
        InitializeComponent();
    }

    private void NewTicket_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new NewTicketDialog();
        dialog.ShowDialog(TopLevel.GetTopLevel(this) as Window ?? this.VisualRoot as Window);
    }

    private void SearchBox_TextChanged(object? sender, RoutedEventArgs e) { /* TODO */ }
    private void Filter_SelectionChanged(object? sender, SelectionChangedEventArgs e) { /* TODO */ }

    private void ResetFilter_Click(object sender, RoutedEventArgs e)
    {
        SearchBox.Text = string.Empty;
        StatusFilter.SelectedIndex = 0;
        PriorityFilter.SelectedIndex = 0;
    }

    private void TicketGrid_DoubleTapped(object? sender, TappedEventArgs e) { /* TODO */ }
    private void TicketGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e) { }
}
