using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HelpDeskPro.Avalonia.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        // TODO: implement login logic
        // On success: open MainWindow, close this
        var main = new MainWindow();
        main.Show();
        Close();
    }
}
