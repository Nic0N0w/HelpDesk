using HelpDeskPro.Wpf.Converters;
using HelpDeskPro.Wpf.Services;
using HelpDeskPro.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace HelpDeskPro.Wpf;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var httpClientHandler = new System.Net.Http.HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            var services = new ServiceCollection();
            services.AddHttpClient<ApiService>()
                .ConfigureHttpClient(client => 
                {
                    client.BaseAddress = new Uri("http://localhost:5000/");
                })
                .ConfigurePrimaryHttpMessageHandler(() => httpClientHandler);

            services.AddSingleton<AuthState>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();

            Services = services.BuildServiceProvider();

            RegisterConverters();

            var login = Services.GetRequiredService<LoginWindow>();
            login.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Starten der Anwendung: {ex.Message}\n\n{ex.StackTrace}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    private void RegisterConverters()
    {
        Resources["StatusToColor"] = new StatusToColorConverter();
        Resources["StatusToBackground"] = new StatusToBackgroundConverter();
        Resources["PriorityToColor"] = new PriorityToColorConverter();
        Resources["BoolToVisibility"] = new BoolToVisibilityConverter();
        Resources["InverseBoolToVisibility"] = new InverseBoolToVisibilityConverter();
        Resources["NullToVisibility"] = new NullToVisibilityConverter();
        Resources["RoleToVisibility"] = new RoleToVisibilityConverter();
    }
}
