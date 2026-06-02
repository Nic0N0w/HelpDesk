using HelpDeskPro.Core.Enums;

namespace HelpDeskPro.Core.Services;
public class StatusTransitionService
{
    private static readonly Dictionary<TicketStatus, TicketStatus[]> _allowed = new()
    {
        { TicketStatus.Open,       [TicketStatus.InProgress] },
        { TicketStatus.InProgress, [TicketStatus.Closed] },
        { TicketStatus.Closed,     [TicketStatus.Open] }, 
    };

    public bool IsAllowed(TicketStatus current, TicketStatus next)
        => _allowed.TryGetValue(current, out var allowed) && allowed.Contains(next);

    public string[] GetAllowed(TicketStatus current)
        => _allowed.TryGetValue(current, out var allowed)
            ? allowed.Select(s => s.ToString()).ToArray()
            : [];

    public (bool ok, string error) Validate(TicketStatus current, TicketStatus next)
    {
        if (current == next) return (false, "Status ist bereits gesetzt.");
        if (!IsAllowed(current, next))
            return (false, $"Übergang von '{current}' nach '{next}' ist nicht erlaubt. Erlaubt: {string.Join(", ", GetAllowed(current))}");
        return (true, string.Empty);
    }
}
