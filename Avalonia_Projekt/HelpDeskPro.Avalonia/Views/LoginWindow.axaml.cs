using Avalonia.Controls;
using Avalonia.Interactivity;
using HelpDeskPro.Avalonia.Models;
using HelpDeskPro.Avalonia.Views;

namespace HelpDeskPro.Avalonia.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        HideError();

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
            var response = await App.ApiService.LoginAsync(email, password);

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

            App.AuthState.Login(userDto, response.Token);

            var main = new MainWindow();
            main.Show();
            Close();
        }
        catch (System.Exception ex)
        {
            ShowError($"Anmeldung fehlgeschlagen: {ex.Message}");
        }
    }

    private void ShowError(string msg)
    {
        ErrorText.Text = msg;
        ErrorBorder.IsVisible = true;
    }

    private void HideError() => ErrorBorder.IsVisible = false;
}
