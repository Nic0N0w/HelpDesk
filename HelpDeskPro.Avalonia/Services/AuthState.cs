using HelpDeskPro.Avalonia.Models;

namespace HelpDeskPro.Avalonia.Services;

public class AuthState
{
    public UserDto? CurrentUser { get; private set; }
    public string? AuthToken { get; private set; }
    public bool IsLoggedIn => CurrentUser is not null && !string.IsNullOrEmpty(AuthToken);

    public void Login(UserDto user, string token)
    {
        CurrentUser = user;
        AuthToken = token;
    }

    public void Logout()
    {
        CurrentUser = null;
        AuthToken = null;
    }
}
