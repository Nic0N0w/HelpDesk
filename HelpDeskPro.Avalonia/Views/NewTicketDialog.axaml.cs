using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HelpDeskPro.Avalonia.Views;

public partial class NewTicketDialog : Window
{
    public NewTicketDialog()
    {
        InitializeComponent();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

    private void Create_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleBox.Text))
        {
            ErrorBorder.IsVisible = true;
            ErrorText.Text = "Bitte einen Titel eingeben.";
            return;
        }
        // TODO: create ticket
        Close();
    }
}
