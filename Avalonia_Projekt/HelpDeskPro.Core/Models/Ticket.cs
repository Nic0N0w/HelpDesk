using HelpDeskPro.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace HelpDeskPro.Core.Models;

public class Ticket
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public Priority Priority { get; set; } = Priority.Medium;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Foreign Keys
    public int CreatedByUserId { get; set; }
    public int? AssignedToUserId { get; set; }

    // Navigation
    public User? CreatedBy { get; set; }
    public User? AssignedTo { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
