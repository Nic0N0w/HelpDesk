using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HelpDeskPro.Avalonia.Views;

public partial class NewTicketDialog : Window
{
    public bool Success { get; private set; }

    public NewTicketDialog()
    {
        InitializeComponent();
    }

    private async void Create_Click(object sender, RoutedEventArgs e)
    {
        ErrorBorder.IsVisible = false;

        if (string.IsNullOrWhiteSpace(TitleBox.Text))
        {
            ErrorText.Text = "Titel ist erforderlich.";
            ErrorBorder.IsVisible = true;
            return;
        }

        var priority = PriorityBox.SelectedIndex; // 0=Low,1=Medium,2=High,3=Critical

        var result = await App.ApiService.CreateTicketAsync(
            TitleBox.Text.Trim(),
            DescBox.Text?.Trim() ?? "",
            priority,
            App.AuthState.CurrentUser!.Id);

        if (result is not null)
        {
            Success = true;
            Close();
        }
        else
        {
            ErrorText.Text = "Fehler beim Erstellen. Ist das Backend erreichbar?";
            ErrorBorder.IsVisible = true;
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
}
