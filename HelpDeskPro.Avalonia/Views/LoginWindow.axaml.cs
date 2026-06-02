using Avalonia.Controls;
using Avalonia.Interactivity;
using HelpDeskPro.Avalonia.Models;
using HelpDeskPro.Avalonia.Services;

namespace HelpDeskPro.Avalonia.Views;

public partial class LoginWindow : Window
{
    private readonly ApiService _api;
    private readonly AuthState _auth;

    // Parameterloser Konstruktor für den Avalonia Designer
    public LoginWindow() : this(App.ApiService, App.AuthState) { }

    public LoginWindow(ApiService api, AuthState auth)
    {
        _api = api;
        _auth = auth;
        InitializeComponent();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        ErrorBorder.IsVisible = false;

        var email = EmailBox.Text?.Trim() ?? "";
        var password = PasswordBox.Text ?? "";

        if (string.IsNullOrWhiteSpace(email))
        {
            ShowError("Bitte E-Mail-Adresse eingeben.");
            return;
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            ShowError("Bitte Passwort eingeben.");
            return;
        }

        try
        {
            var response = await _api.LoginAsync(email, password);
            if (response is null)
            {
                ShowError("Ungültige E-Mail oder Passwort.");
                return;
            }

            var userDto = new UserDto
            {
                Id = response.UserId,
                Name = response.Name,
                Email = response.Email,
                Role = (UserRole)response.Role
            };

            _auth.Login(userDto, response.Token);

            var main = new MainWindow(_api, _auth);
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
        ErrorBorder.IsVisible = true;
    }
}
