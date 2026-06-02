using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using HelpDeskPro.Avalonia.Models;
using HelpDeskPro.Avalonia.Services;
using HelpDeskPro.Avalonia.Views.Pages;

namespace HelpDeskPro.Avalonia.Views;

public partial class MainWindow : Window
{
    private readonly ApiService _api;
    private readonly AuthState _auth;

    public MainWindow() : this(App.ApiService, App.AuthState) { }

    public MainWindow(ApiService api, AuthState auth)
    {
        _api = api;
        _auth = auth;
        InitializeComponent();
        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        var user = _auth.CurrentUser!;
        UserNameText.Text = user.Name;

        if (user.Role == UserRole.Admin)
        {
            RoleBadge.Background = new SolidColorBrush(Color.FromRgb(255, 213, 79));
            RoleText.Text = "Admin";
            RoleText.Foreground = new SolidColorBrush(Color.FromRgb(93, 64, 55));
            BtnAdmin.IsVisible = true;
        }
        else
        {
            RoleBadge.Background = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255));
            RoleText.Text = "Employee";
            RoleText.Foreground = Brushes.White;
            BtnAdmin.IsVisible = false;
        }

        SetActiveButton(BtnAllTickets);
        ContentArea.Content = new TicketListPage(_api, _auth, this);
    }

    public void NavigateToPage(UserControl page, Button? activeBtn = null)
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
            if (!active.Classes.Contains("NavBtnActive"))
                active.Classes.Add("NavBtnActive");
        }
    }

    private void Navigate_AllTickets(object sender, RoutedEventArgs e)
    {
        SetActiveButton(BtnAllTickets);
        ContentArea.Content = new TicketListPage(_api, _auth, this);
    }

    private void Navigate_MyTickets(object sender, RoutedEventArgs e)
    {
        SetActiveButton(BtnMyTickets);
        ContentArea.Content = new MyTicketsPage(_api, _auth, this);
    }

    private void Navigate_Admin(object sender, RoutedEventArgs e)
    {
        SetActiveButton(BtnAdmin);
        ContentArea.Content = new AdminPage(_api, _auth, this);
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        _auth.Logout();
        var login = new LoginWindow(_api, _auth);
        login.Show();
        Close();
    }
}
