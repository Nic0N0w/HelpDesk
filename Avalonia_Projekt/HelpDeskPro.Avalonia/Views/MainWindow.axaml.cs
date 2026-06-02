using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using HelpDeskPro.Avalonia.Models;
using HelpDeskPro.Avalonia.Views.Pages;
using AvaloniaColor = Avalonia.Media.Color;

namespace HelpDeskPro.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Opened += OnOpened;
    }

    private void OnOpened(object? sender, System.EventArgs e)
    {
        var user = App.AuthState.CurrentUser!;
        UserNameText.Text = user.Name;

        if (user.Role == UserRole.Admin)
        {
            RoleBadge.Background = new SolidColorBrush(AvaloniaColor.FromRgb(255, 213, 79));
            RoleText.Text = "Admin";
            RoleText.Foreground = new SolidColorBrush(AvaloniaColor.FromRgb(93, 64, 55));
            BtnAdmin.IsVisible = true;
        }
        else
        {
            RoleBadge.Background = new SolidColorBrush(AvaloniaColor.FromArgb(50, 255, 255, 255));
            RoleText.Text = "Employee";
            RoleText.Foreground = Brushes.White;
            BtnAdmin.IsVisible = false;
        }

        NavigateToPage(new TicketListPage(this), BtnAllTickets);
    }

    public void NavigateToPage(object page, Button? activeBtn = null)
    {
        ContentArea.Content = page;
        SetActiveButton(activeBtn);
    }

    private void SetActiveButton(Button? active)
    {
        foreach (var btn in new[] { BtnAllTickets, BtnMyTickets, BtnAdmin })
        {
            btn.Classes.Remove("NavBtnActive");
            if (!btn.Classes.Contains("NavBtn"))
                btn.Classes.Add("NavBtn");
        }

        if (active != null)
        {
            active.Classes.Remove("NavBtn");
            active.Classes.Add("NavBtnActive");
        }
    }

    private void Navigate_AllTickets(object sender, RoutedEventArgs e)
        => NavigateToPage(new TicketListPage(this), BtnAllTickets);

    private void Navigate_MyTickets(object sender, RoutedEventArgs e)
        => NavigateToPage(new MyTicketsPage(this), BtnMyTickets);

    private void Navigate_Admin(object sender, RoutedEventArgs e)
        => NavigateToPage(new AdminPage(this), BtnAdmin);

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        App.AuthState.Logout();
        var login = new LoginWindow();
        login.Show();
        Close();
    }
}
