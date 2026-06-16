using System.Collections.Generic;
using System;

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
    
    public List<AssigneeDto> Assignees { get; set; } = new();
 
    public List<CommentDto> Comments { get; set; } = new();
    
    public string AssigneesDisplay =>
        Assignees.Count > 0
            ? string.Join(", ", Assignees.Select(a => a.Name))
            : AssignedToName ?? "–";
}

public class AssigneeDto
{
    public int UserId { get; set; } 
    public string Name { get; set; } = "";
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
 
    // Many-to-many
    public List<int> AssignedTicketIds { get; set; } = new();
 
    public override string ToString() => $"{Name} ({Role})";
}
