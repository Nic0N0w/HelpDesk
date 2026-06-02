using HelpDeskPro.Core.Enums;
using HelpDeskPro.Core.Models;

namespace HelpDeskPro.Core.Services;

public class TicketFilter
{
    public TicketStatus? Status { get; set; }
    public Priority? Priority { get; set; }
    public int? AssignedToUserId { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? SearchText { get; set; }
}

public class TicketFilterService
{
    public IEnumerable<Ticket> Apply(IEnumerable<Ticket> tickets, TicketFilter filter)
    {
        var query = tickets.AsQueryable();

        if (filter.Status.HasValue)
            query = query.Where(t => t.Status == filter.Status.Value);

        if (filter.Priority.HasValue)
            query = query.Where(t => t.Priority == filter.Priority.Value);

        if (filter.AssignedToUserId.HasValue)
            query = query.Where(t => t.AssignedToUserId == filter.AssignedToUserId.Value);

        if (filter.CreatedByUserId.HasValue)
            query = query.Where(t => t.CreatedByUserId == filter.CreatedByUserId.Value);

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            var search = filter.SearchText.Trim().ToLower();
            query = query.Where(t =>
                t.Title.ToLower().Contains(search) ||
                t.Description.ToLower().Contains(search));
        }

        return query.OrderByDescending(t => t.CreatedAt);
    }
}
