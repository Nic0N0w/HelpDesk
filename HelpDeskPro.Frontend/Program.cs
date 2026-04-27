using HelpDeskPro.Frontend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// HttpClient für API-Kommunikation
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:49715");
});

// Session-State (einfacher In-Memory Auth-State für M1)
builder.Services.AddScoped<AuthState>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
