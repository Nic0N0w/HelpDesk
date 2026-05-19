using HelpDeskPro.Wpf.Models;
using HelpDeskPro.Wpf.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace HelpDeskPro.Wpf.Views;

public partial class LoginWindow : Window
{
    private readonly ApiService _api;
    private readonly AuthState _auth;

    public LoginWindow(ApiService api, AuthState auth)
    {
        _api = api;
        _auth = auth;
        InitializeComponent();
    }

    private void UserComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) { }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        HideError();

        string email = EmailBox.Text.Trim();
        string password = PasswordBox.Password;

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

            // Create UserDto from response
            var userDto = new UserDto
            {
                Id = response.UserId,
                Name = response.Name,
                Email = response.Email,
                Role = (UserRole)response.Role
            };

            // Store user and token in AuthState
            _auth.Login(userDto, response.Token);

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
