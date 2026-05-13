using System.ComponentModel.DataAnnotations;

namespace HelpDeskPro.Core.Models;

public class Comment
{
    public int Id { get; set; }

    [Required]
    [StringLength(2000)]
    public string Text { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int TicketId { get; set; }
    public Ticket? Ticket { get; set; }

    public int AuthorId { get; set; }
    public User? Author { get; set; }
}
