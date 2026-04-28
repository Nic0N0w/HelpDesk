using HelpDeskPro.Wpf.Models;
using HelpDeskPro.Wpf.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Media;

namespace HelpDeskPro.Wpf.Views;

public partial class MainWindow : Window
{
    private readonly AuthState _auth;
    private readonly ApiService _api;

    public MainWindow(AuthState auth, ApiService api)
    {
        _auth = auth;
        _api = api;
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var user = _auth.CurrentUser!;
            UserNameText.Text = user.Name;

            if (user.Role == UserRole.Admin)
            {
                RoleBadge.Background = new SolidColorBrush(Color.FromRgb(255, 213, 79));
                RoleText.Text = "Admin";
                RoleText.Foreground = new SolidColorBrush(Color.FromRgb(93, 64, 55));
                BtnAdmin.Visibility = Visibility.Visible;
            }
            else
            {
                RoleBadge.Background = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255));
                RoleText.Text = "Employee";
                RoleText.Foreground = Brushes.White;
                BtnAdmin.Visibility = Visibility.Collapsed;
            }

            NavigateToPage(new TicketListPage(_api, _auth, this), BtnAllTickets);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Laden des Fensters: {ex.Message}\n\n{ex.StackTrace}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void NavigateToPage(System.Windows.Controls.Page page, System.Windows.Controls.Button? activeBtn = null)
    {
        ContentFrame.Navigate(page);
        // Reset all nav buttons
        foreach (var btn in new[] { BtnAllTickets, BtnMyTickets, BtnAdmin })
            btn.Tag = null;
        if (activeBtn != null) activeBtn.Tag = "active";
    }

    private void Navigate_AllTickets(object sender, RoutedEventArgs e)
        => NavigateToPage(new TicketListPage(_api, _auth, this), BtnAllTickets);

    private void Navigate_MyTickets(object sender, RoutedEventArgs e)
        => NavigateToPage(new MyTicketsPage(_api, _auth, this), BtnMyTickets);

    private void Navigate_Admin(object sender, RoutedEventArgs e)
        => NavigateToPage(new AdminPage(_api, _auth, this), BtnAdmin);

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        _auth.Logout();
        var login = App.Services.GetRequiredService<LoginWindow>();
        login.Show();
        Close();
    }
}
