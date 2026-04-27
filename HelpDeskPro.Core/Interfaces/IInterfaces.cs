using HelpDeskPro.Core.Enums;
using HelpDeskPro.Core.Models;

namespace HelpDeskPro.Core.Interfaces;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(int id);
    Task<IEnumerable<Ticket>> GetByUserIdAsync(int userId);
    Task<Ticket> CreateAsync(Ticket ticket);
    Task<Ticket> UpdateAsync(Ticket ticket);
    Task<IEnumerable<Ticket>> GetAllAsync();
}

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
}

public interface ICommentRepository
{
    Task<Comment> AddAsync(Comment comment);
    Task<IEnumerable<Comment>> GetByTicketIdAsync(int ticketId);
}

public interface IEmailService
{
    Task SendTicketCreatedAsync(Ticket ticket);
    Task SendStatusChangedAsync(Ticket ticket, TicketStatus oldStatus);
    Task SendCommentAddedAsync(Ticket ticket, Comment comment);
}
