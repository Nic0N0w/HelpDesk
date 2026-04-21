using HelpDeskPro.Core.Enums;
using HelpDeskPro.Core.Interfaces;
using HelpDeskPro.Core.Models;
using HelpDeskPro.Infrastructure;
using HelpDeskPro.Infrastructure.Data;
using HelpDeskPro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Services ────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("HelpDeskProDb"));

builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailService, ConsoleEmailService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "HelpDesk Pro API", Version = "v1" });
});

// CORS für Frontend (Blazor läuft auf anderem Port)
builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// ── Seed-Daten ──────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User { Id = 1, Name = "Admin Anna", Email = "anna@firma.at", Role = UserRole.Admin },
            new User { Id = 2, Name = "Max Mustermann", Email = "max@firma.at", Role = UserRole.Employee }
        );
        db.Tickets.AddRange(
            new Ticket
            {
                Id = 1, Title = "Drucker reagiert nicht", Description = "3. OG, Raum 312",
                Priority = Priority.High, Status = TicketStatus.Open, CreatedByUserId = 2
            },
            new Ticket
            {
                Id = 2, Title = "VPN-Zugang einrichten", Description = "Neuer Mitarbeiter braucht VPN",
                Priority = Priority.Medium, Status = TicketStatus.Open, CreatedByUserId = 2
            }
        );
        db.SaveChanges();
    }
}

// ── Middleware ──────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HelpDesk Pro v1"));
app.UseCors();
app.MapControllers();

app.Run();
