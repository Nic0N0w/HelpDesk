using System.ComponentModel.DataAnnotations;

namespace HelpDeskPro.Wpf.Models;

public enum TicketStatus { Open, InProgress, Closed }
public enum Priority { Low, Medium, High, Critical }
public enum UserRole { Employee, Admin }

public class TicketDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = "";

    [StringLength(2000)]
    public string Description { get; set; } = "";

    public TicketStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int CreatedByUserId { get; set; }

    [Required]
    [StringLength(100)]
    public string CreatedByName { get; set; } = "";

    public int? AssignedToUserId { get; set; }

    [StringLength(100)]
    public string? AssignedToName { get; set; }

    public List<CommentDto> Comments { get; set; } = new();
}

public class CommentDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(2000)]
    public string Text { get; set; } = "";

    public DateTime CreatedAt { get; set; }
    public int AuthorId { get; set; }

    [Required]
    [StringLength(100)]
    public string AuthorName { get; set; } = "";
}

public class UserDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = "";

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = "";

    public UserRole Role { get; set; }

    public override string ToString() => $"{Name} ({Role})";
}
