namespace HelpDeskPro.Avalonia.Models;

public enum TicketStatus { Open, InProgress, Closed }
public enum Priority { Low, Medium, High, Critical }
public enum UserRole { Employee, Admin }

public class LoginResponse
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Role { get; set; }
    public string Token { get; set; } = string.Empty;
}

public class TicketDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public TicketStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int CreatedByUserId { get; set; }
    public string CreatedByName { get; set; } = "";
    public int? AssignedToUserId { get; set; }
    public string? AssignedToName { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
}

public class CommentDto
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = "";
}

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public UserRole Role { get; set; }

    public override string ToString() => $"{Name} ({Role})";
}
