using System.Net.Http;
using System.Windows;
using HelpDeskPro.Client.Wpf.Services;
using HelpDeskPro.Client.Wpf.ViewModels;

namespace HelpDeskPro.Client.Wpf;

public partial class App : Application
{
    private readonly ApiService _apiService;
    private readonly MainViewModel _mainViewModel;

    public App()
    {
        // BaseAddress assumes API runs on localhost:5000 (adjust if needed)
        var http = new HttpClient { BaseAddress = new Uri("https://localhost:5001/") };
        _apiService = new ApiService(http);
        _mainViewModel = new MainViewModel(_apiService);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var main = new MainWindow { DataContext = _mainViewModel };
        main.Show();
    }
}
