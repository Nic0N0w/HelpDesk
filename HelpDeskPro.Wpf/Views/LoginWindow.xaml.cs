using HelpDeskPro.Wpf.Models;
using HelpDeskPro.Wpf.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace HelpDeskPro.Wpf.Views;

public partial class LoginWindow : Window
{
    private readonly ApiService _api;
    private readonly AuthState _auth;
    private List<UserDto> _users = new();

    public LoginWindow(ApiService api, AuthState auth)
    {
        _api = api;
        _auth = auth;
        InitializeComponent();
        Loaded += async (_, _) => await LoadUsers();
    }

    private async Task LoadUsers()
    {
        try
        {
            _users = await _api.GetUsersAsync() ?? new();
            UserComboBox.ItemsSource = _users;
            if (_users.Count > 0) UserComboBox.SelectedIndex = 0;
        }
        catch
        {
            ShowError("API nicht erreichbar. Bitte Backend starten: cd HelpDeskPro.Api && dotnet run");
        }
    }

    private void UserComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        HideError();

        if (UserComboBox.SelectedItem is not UserDto user)
        {
            ShowError("Bitte einen Benutzer auswählen."); return;
        }
        if (string.IsNullOrWhiteSpace(PasswordBox.Password))
        {
            ShowError("Bitte ein Passwort eingeben."); return;
        }

        try
        {
            _auth.Login(user);

            var main = App.Services.GetRequiredService<MainWindow>();
            main.Show();
            Close();
        }
        catch (Exception ex)
        {
            ShowError($"Anmeldung fehlgeschlagen: {ex.Message}");
        }
    }

    private void ShowError(string msg)
    {
        ErrorText.Text = msg;
        ErrorBorder.Visibility = Visibility.Visible;
    }

    private void HideError() => ErrorBorder.Visibility = Visibility.Collapsed;
}
