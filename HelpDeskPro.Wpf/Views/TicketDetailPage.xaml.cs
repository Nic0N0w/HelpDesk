using HelpDeskPro.Wpf.Models;
using HelpDeskPro.Wpf.Services;
using System.Windows;
using System.Windows.Controls;

namespace HelpDeskPro.Wpf.Views;

public partial class TicketDetailPage : Page
{
    private readonly ApiService _api;
    private readonly AuthState _auth;
    private readonly MainWindow _main;
    private readonly int _ticketId;
    private TicketDto? _ticket;
    private List<UserDto> _users = new();

    public TicketDetailPage(ApiService api, AuthState auth, MainWindow main, int ticketId)
    {
        _api = api; _auth = auth; _main = main; _ticketId = ticketId;
        InitializeComponent();
        Loaded += async (_, _) => await LoadAll();
    }

    private async Task LoadAll()
    {
        _ticket = await _api.GetTicketAsync(_ticketId);
        _users = await _api.GetUsersAsync() ?? new();

        if (_ticket is null) return;
        RenderTicket();
    }

    private void RenderTicket()
    {
        var t = _ticket!;
        TitleText.Text = $"Ticket #{t.Id} – {t.Title}";
        DescText.Text = string.IsNullOrWhiteSpace(t.Description) ? "(keine Beschreibung)" : t.Description;

        // Details-Grid
        DetailsGrid.Children.Clear();
        DetailsGrid.RowDefinitions.Clear();
        AddDetailRow("Status", t.Status.ToString());
        AddDetailRow("Priorität", t.Priority.ToString());
        AddDetailRow("Erstellt von", t.CreatedByName);
        AddDetailRow("Zugewiesen an", t.AssignedToName ?? "–");
        AddDetailRow("Erstellt am", t.CreatedAt.ToString("dd.MM.yyyy HH:mm"));
        if (t.UpdatedAt.HasValue)
            AddDetailRow("Geändert", t.UpdatedAt.Value.ToString("dd.MM.yyyy HH:mm"));

        // Status Combo
        StatusCombo.SelectedIndex = t.Status switch
        {
            TicketStatus.Open => 0,
            TicketStatus.InProgress => 1,
            TicketStatus.Closed => 2,
            _ => 0
        };

        // Kommentare
        CommentHeader.Text = $"Kommentare ({t.Comments.Count})";
        CommentsList.ItemsSource = t.Comments.OrderBy(c => c.CreatedAt).ToList();

        // Admin: Zuweisung
        if (_auth.CurrentUser?.Role == UserRole.Admin)
        {
            AssignPanel.Visibility = Visibility.Visible;
            AssignCombo.ItemsSource = _users;
            if (t.AssignedToUserId.HasValue)
                AssignCombo.SelectedItem = _users.FirstOrDefault(u => u.Id == t.AssignedToUserId);
        }
    }

    private void AddDetailRow(string label, string value)
    {
        var row = DetailsGrid.RowDefinitions.Count;
        DetailsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var lbl = new TextBlock
        {
            Text = label, FontWeight = System.Windows.FontWeights.SemiBold,
            FontSize = 12, Foreground = System.Windows.Media.Brushes.Gray,
            Margin = new Thickness(0, 4, 0, 4)
        };
        var val = new TextBlock
        {
            Text = value, FontSize = 13,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 4, 0, 4)
        };

        Grid.SetRow(lbl, row); Grid.SetColumn(lbl, 0);
        Grid.SetRow(val, row); Grid.SetColumn(val, 1);

        if (DetailsGrid.ColumnDefinitions.Count == 0)
        {
            DetailsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            DetailsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        DetailsGrid.Children.Add(lbl);
        DetailsGrid.Children.Add(val);

        if (row > 0)
        {
            var sep = new Separator { Foreground = System.Windows.Media.Brushes.LightGray };
            Grid.SetRow(sep, row); Grid.SetColumnSpan(sep, 2);
        }
    }

    private async void ChangeStatus_Click(object sender, RoutedEventArgs e)
    {
        StatusErrorBorder.Visibility = Visibility.Collapsed;
        StatusSuccessBorder.Visibility = Visibility.Collapsed;

        var newStatus = (StatusCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Open";
        var (updated, error) = await _api.UpdateStatusAsync(_ticketId, newStatus);
        if (updated is not null)
        {
            _ticket = updated;
            RenderTicket();
            StatusSuccessText.Text = $"Status erfolgreich auf '{newStatus}' gesetzt.";
            StatusSuccessBorder.Visibility = Visibility.Visible;
        }
        else
        {
            StatusErrorText.Text = error ?? "Statusänderung fehlgeschlagen.";
            StatusErrorBorder.Visibility = Visibility.Visible;
        }
    }

    private async void Assign_Click(object sender, RoutedEventArgs e)
    {
        if (AssignCombo.SelectedItem is not UserDto user) return;
        var updated = await _api.AssignTicketAsync(_ticketId, user.Id);
        if (updated is not null) { _ticket = updated; RenderTicket(); }
    }

    private async void AddComment_Click(object sender, RoutedEventArgs e)
    {
        CommentErrorBorder.Visibility = Visibility.Collapsed;
        var text = CommentBox.Text.Trim();
        if (string.IsNullOrEmpty(text))
        {
            CommentErrorText.Text = "Kommentar darf nicht leer sein.";
            CommentErrorBorder.Visibility = Visibility.Visible;
            return;
        }
        var ok = await _api.AddCommentAsync(_ticketId, text, _auth.CurrentUser!.Id);
        if (ok)
        {
            CommentBox.Text = "";
            _ticket = await _api.GetTicketAsync(_ticketId);
            RenderTicket();
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
        => _main.NavigateToPage(new TicketListPage(_api, _auth, _main), null);
}
