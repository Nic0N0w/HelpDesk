using Avalonia.Controls;
using Avalonia.Interactivity;
using HelpDeskPro.Avalonia.Services;

namespace HelpDeskPro.Avalonia.Views;

public partial class NewTicketDialog : Window
{
    private readonly ApiService _api;
    private readonly AuthState _auth;

    public bool DialogResult { get; private set; }

    public NewTicketDialog() : this(App.ApiService, App.AuthState) { }

    public NewTicketDialog(ApiService api, AuthState auth)
    {
        _api = api;
        _auth = auth;
        InitializeComponent();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
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
        var result = await _api.CreateTicketAsync(
            TitleBox.Text.Trim(),
            DescBox.Text?.Trim() ?? "",
            priority,
            _auth.CurrentUser!.Id);

        if (result is not null)
        {
            DialogResult = true;
            Close();
        }
        else
        {
            ErrorText.Text = "Fehler beim Erstellen. Ist das Backend erreichbar?";
            ErrorBorder.IsVisible = true;
        }
    }
}
