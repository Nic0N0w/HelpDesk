using Avalonia.Controls;
using Avalonia.Interactivity;
using HelpDeskPro.Avalonia.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpDeskPro.Avalonia.Views.Pages;

public partial class TicketDetailPage : UserControl
{
    private readonly MainWindow _main;
    private readonly int _ticketId;
    private TicketDto? _ticket;
    private List<UserDto> _users = new();

    public TicketDetailPage(MainWindow main, int ticketId)
    {
        _main = main;
        _ticketId = ticketId;
        InitializeComponent();
        Loaded += async (_, _) => await LoadAll();
    }

    private async Task LoadAll()
    {
        _ticket = await App.ApiService.GetTicketAsync(_ticketId);
        _users = await App.ApiService.GetUsersAsync() ?? new();

        if (_ticket is null) return;
        RenderTicket();
    }

    private void RenderTicket()
    {
        var t = _ticket!;
        TitleText.Text = $"Ticket #{t.Id} – {t.Title}";
        DescText.Text = string.IsNullOrWhiteSpace(t.Description) ? "(keine Beschreibung)" : t.Description;

        StatusText.Text = t.Status.ToString();
        PriorityText.Text = t.Priority.ToString();
        CreatedByText.Text = t.CreatedByName;
        AssignedToText.Text = t.AssigneesDisplay;
        CreatedAtText.Text = t.CreatedAt.ToString("dd.MM.yyyy HH:mm");
        UpdatedAtText.Text = t.UpdatedAt.HasValue ? t.UpdatedAt.Value.ToString("dd.MM.yyyy HH:mm") : "–";

        // Status Combo
        StatusCombo.SelectedIndex = t.Status switch
        {
            TicketStatus.Open => 0,
            TicketStatus.InProgress => 1,
            TicketStatus.Closed => 2,
            _ => 0
        };

        // Kommentare
        CommentHeader.Text = $"💬 Kommentare ({t.Comments.Count})";
        CommentsList.ItemsSource = t.Comments.OrderBy(c => c.CreatedAt).ToList();

        // Admin: Zuordnung (n:m)
        if (App.AuthState.CurrentUser?.Role == UserRole.Admin)
        {
            AssignPanel.IsVisible = true;

            // Aktuelle Assignees anzeigen
            AssigneesListControl.ItemsSource = t.Assignees.ToList();

            // Nur User anbieten, die noch nicht zugeordnet sind
            var assignedIds = t.Assignees.Select(a => a.UserId).ToHashSet();
            AddAssignCombo.ItemsSource = _users.Where(u => !assignedIds.Contains(u.Id)).ToList();
            AddAssignCombo.SelectedIndex = -1;
        }
        else
        {
            AssignPanel.IsVisible = false;
        }
    }

    private async void ChangeStatus_Click(object sender, RoutedEventArgs e)
    {
        StatusErrorBorder.IsVisible = false;
        StatusSuccessBorder.IsVisible = false;

        var newStatus = (StatusCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Open";
        var (updated, error) = await App.ApiService.UpdateStatusAsync(_ticketId, newStatus);

        if (updated is not null)
        {
            _ticket = updated;
            RenderTicket();
            StatusSuccessText.Text = $"Status erfolgreich auf '{newStatus}' gesetzt.";
            StatusSuccessBorder.IsVisible = true;
        }
        else
        {
            StatusErrorText.Text = error ?? "Statusänderung fehlgeschlagen.";
            StatusErrorBorder.IsVisible = true;
        }
    }

    private async void AddAssignee_Click(object sender, RoutedEventArgs e)
    {
        if (AddAssignCombo.SelectedItem is not UserDto user) return;
        var updated = await App.ApiService.AddAssigneeAsync(_ticketId, user.Id);
        if (updated is not null) { _ticket = updated; RenderTicket(); }
    }

    private async void RemoveAssignee_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        if (btn.Tag is not int userId) return;
        var updated = await App.ApiService.RemoveAssigneeAsync(_ticketId, userId);
        if (updated is not null) { _ticket = updated; RenderTicket(); }
    }

    private async void AddComment_Click(object sender, RoutedEventArgs e)
    {
        CommentErrorBorder.IsVisible = false;
        var text = CommentBox.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(text))
        {
            CommentErrorText.Text = "Kommentar darf nicht leer sein.";
            CommentErrorBorder.IsVisible = true;
            return;
        }

        var ok = await App.ApiService.AddCommentAsync(_ticketId, text, App.AuthState.CurrentUser!.Id);
        if (ok)
        {
            CommentBox.Text = "";
            _ticket = await App.ApiService.GetTicketAsync(_ticketId);
            RenderTicket();
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
        => _main.NavigateToPage(new TicketListPage(_main), null);
}
