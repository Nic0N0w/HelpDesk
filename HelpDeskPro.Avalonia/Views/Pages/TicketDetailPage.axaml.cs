using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using HelpDeskPro.Avalonia.Models;
using HelpDeskPro.Avalonia.Services;

namespace HelpDeskPro.Avalonia.Views.Pages;

public partial class TicketDetailPage : UserControl
{
    private readonly ApiService _api;
    private readonly AuthState _auth;
    private readonly MainWindow _main;
    private readonly int _ticketId;
    private TicketDto? _ticket;
    private List<UserDto> _users = new();

    public TicketDetailPage() : this(App.ApiService, App.AuthState, null!, 0) { }

    public TicketDetailPage(ApiService api, AuthState auth, MainWindow main, int ticketId)
    {
        _api = api;
        _auth = auth;
        _main = main;
        _ticketId = ticketId;
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

        // Detail-Felder befüllen
        StatusText.Text = t.Status.ToString();
        PriorityText.Text = t.Priority.ToString();
        CreatedByText.Text = t.CreatedByName;
        AssignedToText.Text = t.AssignedToName ?? "–";

        // Status Combo
        StatusCombo.SelectedIndex = t.Status switch
        {
            TicketStatus.Open => 0,
            TicketStatus.InProgress => 1,
            TicketStatus.Closed => 2,
            _ => 0
        };

        // Kommentare
        var sorted = t.Comments.OrderBy(c => c.CreatedAt).ToList();
        CommentHeader.Text = $"💬 Kommentare ({sorted.Count})";
        CommentsList.ItemsSource = sorted;

        // Admin: Zuweisung anzeigen
        if (_auth.CurrentUser?.Role == UserRole.Admin)
        {
            AssignPanel.IsVisible = true;
            AssignCombo.ItemsSource = _users;
            if (t.AssignedToUserId.HasValue)
                AssignCombo.SelectedItem = _users.FirstOrDefault(u => u.Id == t.AssignedToUserId);
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
        var (updated, error) = await _api.UpdateStatusAsync(_ticketId, newStatus);

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

    private async void Assign_Click(object sender, RoutedEventArgs e)
    {
        if (AssignCombo.SelectedItem is not UserDto user) return;
        var updated = await _api.AssignTicketAsync(_ticketId, user.Id);
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
