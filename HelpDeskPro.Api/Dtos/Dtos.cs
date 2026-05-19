using HelpDeskPro.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace HelpDeskPro.Api.Dtos;

// Auth
public record LoginRequest(
    [Required]
    [EmailAddress]
    string Email,
    [Required]
    [StringLength(100, MinimumLength = 6)]
    string Password
);

public record LoginResponse(
    int UserId,
    [Required]
    [StringLength(100)]
    string Name,
    [Required]
    [EmailAddress]
    string Email,
    UserRole Role,
    [Required]
    string Token
);

// Ticket
public record CreateTicketRequest(
    [Required]
    [StringLength(200)]
    string Title,
    [StringLength(2000)]
    string Description,
    Priority Priority,
    [Range(1, int.MaxValue)]
    int CreatedByUserId
);

public record UpdateStatusRequest(
    [Required]
    TicketStatus NewStatus
);

public record TicketResponse(
    int Id,
    [Required]
    [StringLength(200)]
    string Title,
    [StringLength(2000)]
    string Description,
    TicketStatus Status,
    Priority Priority,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int CreatedByUserId,
    [Required]
    [StringLength(100)]
    string CreatedByName,
    int? AssignedToUserId,
    [StringLength(100)]
    string? AssignedToName,
    IEnumerable<CommentResponse> Comments
);

// Comment
public record AddCommentRequest(
    [Required]
    [StringLength(2000)]
    string Text,
    [Range(1, int.MaxValue)]
    int AuthorId
);

public record AssignTicketRequest(
    [Range(1, int.MaxValue)]
    int AssignedToUserId
);

public record CommentResponse(
    int Id,
    [Required]
    [StringLength(2000)]
    string Text,
    DateTime CreatedAt,
    int AuthorId,
    [Required]
    [StringLength(100)]
    string AuthorName
);

// User
public record UserResponse(
    int Id,
    [Required]
    [StringLength(100)]
    string Name,
    [Required]
    [EmailAddress]
    [StringLength(255)]
    string Email,
    UserRole Role
);

// Filter
public record TicketFilterRequest(
    TicketStatus? Status,
    Priority? Priority,
    int? AssignedToUserId,
    int? CreatedByUserId,
    [StringLength(100)]
    string? SearchText
);
