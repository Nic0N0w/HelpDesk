using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HelpDeskPro.Avalonia.Services;
using HelpDeskPro.Avalonia.Views;

namespace HelpDeskPro.Avalonia;

public partial class App : Application
{
    public static AuthState AuthState { get; } = new AuthState();
    public static ApiService ApiService { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ApiService = new ApiService(AuthState);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new LoginWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
