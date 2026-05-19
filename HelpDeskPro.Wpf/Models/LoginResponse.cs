namespace HelpDeskPro.Wpf.Models;

public class LoginResponse
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Role { get; set; }
    public string Token { get; set; } = string.Empty;
}
