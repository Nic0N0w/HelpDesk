using HelpDeskPro.Core.Enums;
using HelpDeskPro.Core.Interfaces;
using HelpDeskPro.Core.Models;

namespace HelpDeskPro.Infrastructure;

/// <summary>
/// Mock-Implementierung: Gibt E-Mails in die Konsole aus.
/// Einfach durch echten SMTP-Service ersetzbar (IEmailService implementieren).
/// </summary>
public class ConsoleEmailService : IEmailService
{
    public Task SendTicketCreatedAsync(Ticket ticket)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[EMAIL] An: {ticket.CreatedBy?.Email ?? "user@firma.at"}");
        Console.WriteLine($"        Betreff: Ticket #{ticket.Id} erstellt: \"{ticket.Title}\"");
        Console.WriteLine($"        Priorität: {ticket.Priority} | Status: {ticket.Status}");
        Console.ResetColor();
        return Task.CompletedTask;
    }

    public Task SendStatusChangedAsync(Ticket ticket, TicketStatus oldStatus)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[EMAIL] An: {ticket.CreatedBy?.Email ?? "user@firma.at"}");
        Console.WriteLine($"        Betreff: Ticket #{ticket.Id} – Status geändert");
        Console.WriteLine($"        {oldStatus} → {ticket.Status}");
        Console.ResetColor();
        return Task.CompletedTask;
    }

    public Task SendCommentAddedAsync(Ticket ticket, Comment comment)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[EMAIL] An: {ticket.CreatedBy?.Email ?? "user@firma.at"}");
        Console.WriteLine($"        Betreff: Neuer Kommentar zu Ticket #{ticket.Id}: \"{ticket.Title}\"");
        Console.WriteLine($"        Von: {comment.Author?.Name} – \"{comment.Text}\"");
        Console.ResetColor();
        return Task.CompletedTask;
    }
}
