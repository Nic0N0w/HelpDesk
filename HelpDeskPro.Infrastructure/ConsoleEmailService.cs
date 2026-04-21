using HelpDeskPro.Core.Enums;
using HelpDeskPro.Core.Interfaces;
using HelpDeskPro.Core.Models;

namespace HelpDeskPro.Infrastructure;

/// <summary>
/// Mock-Implementierung: Gibt E-Mails in die Konsole aus.
/// In M2 durch echten SMTP-Service ersetzbar.
/// </summary>
public class ConsoleEmailService : IEmailService
{
    public Task SendTicketCreatedAsync(Ticket ticket)
    {
        Console.WriteLine($"[EMAIL MOCK] Neues Ticket #{ticket.Id}: \"{ticket.Title}\" erstellt von UserId {ticket.CreatedByUserId}.");
        return Task.CompletedTask;
    }

    public Task SendStatusChangedAsync(Ticket ticket, TicketStatus oldStatus)
    {
        Console.WriteLine($"[EMAIL MOCK] Ticket #{ticket.Id} Status: {oldStatus} → {ticket.Status}");
        return Task.CompletedTask;
    }
}
