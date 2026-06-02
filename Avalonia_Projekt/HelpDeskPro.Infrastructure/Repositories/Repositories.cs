using HelpDeskPro.Core.Interfaces;
using HelpDeskPro.Core.Models;
using HelpDeskPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskPro.Infrastructure.Repositories;

public class TicketRepository(AppDbContext db) : ITicketRepository
{
    public async Task<Ticket?> GetByIdAsync(int id) =>
        await db.Tickets
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Include(t => t.Comments).ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<Ticket>> GetByUserIdAsync(int userId) =>
        await db.Tickets
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Where(t => t.CreatedByUserId == userId || t.AssignedToUserId == userId)
            .ToListAsync();

    public async Task<IEnumerable<Ticket>> GetAllAsync() =>
        await db.Tickets
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Include(t => t.Comments).ThenInclude(c => c.Author)
            .ToListAsync();

    public async Task<Ticket> CreateAsync(Ticket ticket)
    {
        db.Tickets.Add(ticket);
        await db.SaveChangesAsync();
        return ticket;
    }

    public async Task<Ticket> UpdateAsync(Ticket ticket)
    {
        ticket.UpdatedAt = DateTime.UtcNow;
        db.Tickets.Update(ticket);
        await db.SaveChangesAsync();
        return ticket;
    }
}

public class UserRepository(AppDbContext db) : IUserRepository
{
    public async Task<User?> GetByIdAsync(int id) =>
        await db.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetByEmailAsync(string email) =>
        await db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<IEnumerable<User>> GetAllAsync() =>
        await db.Users.ToListAsync();
}

public class CommentRepository(AppDbContext db) : ICommentRepository
{
    public async Task<Comment> AddAsync(Comment comment)
    {
        db.Comments.Add(comment);
        await db.SaveChangesAsync();
        return comment;
    }

    public async Task<IEnumerable<Comment>> GetByTicketIdAsync(int ticketId) =>
        await db.Comments
            .Include(c => c.Author)
            .Where(c => c.TicketId == ticketId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
}
