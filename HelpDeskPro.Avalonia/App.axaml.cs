using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HelpDeskPro.Avalonia.Services;
using HelpDeskPro.Avalonia.Views;
using System;
using System.Net.Http;

namespace HelpDeskPro.Avalonia;

public partial class App : Application
{
    // Statische Singletons – einfacher als DI-Container in Avalonia Desktop
    public static ApiService ApiService { get; private set; } = null!;
    public static AuthState AuthState { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Services initialisieren
        AuthState = new AuthState();

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        var http = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5000/")
        };
        ApiService = new ApiService(http, AuthState);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new LoginWindow(ApiService, AuthState);
        }

        base.OnFrameworkInitializationCompleted();
    }
}
