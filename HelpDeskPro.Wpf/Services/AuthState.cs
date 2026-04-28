using HelpDeskPro.Wpf.Models;

namespace HelpDeskPro.Wpf.Services;

public class AuthState
{
    public UserDto? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser is not null;

    public void Login(UserDto user) => CurrentUser = user;
    public void Logout() => CurrentUser = null;
}
