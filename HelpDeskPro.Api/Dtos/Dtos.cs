using HelpDeskPro.Core.Enums;

namespace HelpDeskPro.Api.Dtos;
public record CreateTicketRequest(
    string Title,
    string Description,
    Priority Priority,
    int CreatedByUserId
);

public record UpdateStatusRequest(TicketStatus NewStatus);

public record TicketResponse(
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
    string? AssignedToName,
    IEnumerable<CommentResponse> Comments
);

public record CommentResponse(
    int Id,
    string Text,
    DateTime CreatedAt,
    int AuthorId,
    string AuthorName
);
public record UserResponse(int Id, string Name, string Email, UserRole Role);