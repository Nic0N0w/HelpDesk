using HelpDeskPro.Core.Enums;

namespace HelpDeskPro.Client.Wpf.Models;

public record TicketResponseDto(
    int Id,
    string Title,
    string Description,
    TicketStatus Status,
    Priority Priority,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int CreatedByUserId,
    string CreatedByName,
    int? AssignedToUserId,
    string? AssignedToName
);
