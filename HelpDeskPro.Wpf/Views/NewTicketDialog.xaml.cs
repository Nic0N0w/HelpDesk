using HelpDeskPro.Wpf.Services;
using System.Windows;
using System.Windows.Controls;

namespace HelpDeskPro.Wpf.Views;

public partial class NewTicketDialog : Window
{
    private readonly ApiService _api;
    private readonly AuthState _auth;

    public NewTicketDialog(ApiService api, AuthState auth)
    {
        _api = api; _auth = auth;
        InitializeComponent();
    }

    private async void Create_Click(object sender, RoutedEventArgs e)
    {
        ErrorBorder.Visibility = Visibility.Collapsed;

        if (string.IsNullOrWhiteSpace(TitleBox.Text))
        {
            ErrorText.Text = "Titel ist erforderlich.";
            ErrorBorder.Visibility = Visibility.Visible;
            return;
        }

        var priority = PriorityBox.SelectedIndex; // 0=Low,1=Medium,2=High,3=Critical
        var result = await _api.CreateTicketAsync(
            TitleBox.Text.Trim(),
            DescBox.Text.Trim(),
            priority,
            _auth.CurrentUser!.Id);

        if (result is not null)
        {
            DialogResult = true;
        }
        else
        {
            ErrorText.Text = "Fehler beim Erstellen. Ist das Backend erreichbar?";
            ErrorBorder.Visibility = Visibility.Visible;
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
