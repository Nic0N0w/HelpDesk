using Avalonia.Controls;
using Avalonia.Interactivity;
using HelpDeskPro.Avalonia.Views.Pages;

namespace HelpDeskPro.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // Show TicketListPage by default
        ContentArea.Content = new TicketListPage();
    }

    private void SetActiveButton(Button active)
    {
        BtnAllTickets.Classes.Remove("NavBtnActive");
        BtnMyTickets.Classes.Remove("NavBtnActive");
        BtnAdmin.Classes.Remove("NavBtnActive");
        BtnAllTickets.Classes.Add("NavBtn");
        BtnMyTickets.Classes.Add("NavBtn");
        BtnAdmin.Classes.Add("NavBtn");

        active.Classes.Remove("NavBtn");
        active.Classes.Add("NavBtnActive");
    }

    private void Navigate_AllTickets(object sender, RoutedEventArgs e)
    {
        SetActiveButton(BtnAllTickets);
        ContentArea.Content = new TicketListPage();
    }

    private void Navigate_MyTickets(object sender, RoutedEventArgs e)
    {
        SetActiveButton(BtnMyTickets);
        ContentArea.Content = new MyTicketsPage();
    }

    private void Navigate_Admin(object sender, RoutedEventArgs e)
    {
        SetActiveButton(BtnAdmin);
        ContentArea.Content = new AdminPage();
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        var login = new LoginWindow();
        login.Show();
        Close();
    }
}
