using HelpDeskPro.Core.Enums;

namespace HelpDeskPro.Api.Dtos;

// ── Ticket ──────────────────────────────────────────────
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

// ── Comment ─────────────────────────────────────────────
public record AddCommentRequest(string Text, int AuthorId);

public record AssignTicketRequest(int AssignedToUserId);

public record CommentResponse(
    int Id,
    string Text,
    DateTime CreatedAt,
    int AuthorId,
    string AuthorName
);

// ── User ────────────────────────────────────────────────
public record UserResponse(int Id, string Name, string Email, UserRole Role);

// ── Filter ──────────────────────────────────────────────
public record TicketFilterRequest(
    TicketStatus? Status,
    Priority? Priority,
    int? AssignedToUserId,
    int? CreatedByUserId,
    string? SearchText
);
