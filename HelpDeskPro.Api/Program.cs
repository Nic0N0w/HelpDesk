using HelpDeskPro.Core.Enums;
using HelpDeskPro.Core.Interfaces;
using HelpDeskPro.Core.Models;
using HelpDeskPro.Core.Services;
using HelpDeskPro.Infrastructure;
using HelpDeskPro.Infrastructure.Data;
using HelpDeskPro.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("HelpDeskPro")));

builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IEmailService, ConsoleEmailService>();
builder.Services.AddScoped<StatusTransitionService>();
builder.Services.AddScoped<TicketFilterService>();

// Add JWT Service
var jwtSecret = builder.Configuration["JwtSecret"] ?? "your-secret-key-min-32-characters-long!";
builder.Services.AddScoped(sp => new JwtService(jwtSecret, expirationMinutes: 1440));

// Add JWT Authentication
var key = Encoding.UTF8.GetBytes(jwtSecret);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = "HelpDeskPro",
            ValidateAudience = true,
            ValidAudience = "HelpDeskProClient",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "HelpDesk Pro API", Version = "v1" }));

builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
//HTTP PORT
app.Urls.Add("http://0.0.0.0:5000");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
    if (!db.Users.Any())
    {
        // Demo Passwords: Password123!, Password456!, Password789!, Password000!
        db.Users.AddRange(
            new User 
            { 
                Id = 1, 
                Name = "Admin Anna", 
                Email = "anna@firma.at", 
                Role = UserRole.Admin,
                PasswordHash = PasswordService.HashPassword("Password123!")
            },
            new User 
            { 
                Id = 2, 
                Name = "Max Mustermann", 
                Email = "max@firma.at", 
                Role = UserRole.Employee,
                PasswordHash = PasswordService.HashPassword("Password456!")
            },
            new User 
            { 
                Id = 3, 
                Name = "Lisa Maier", 
                Email = "lisa@firma.at", 
                Role = UserRole.Employee,
                PasswordHash = PasswordService.HashPassword("Password789!")
            },
            new User 
            { 
                Id = 4, 
                Name = "Tom Berger", 
                Email = "tom@firma.at", 
                Role = UserRole.Employee,
                PasswordHash = PasswordService.HashPassword("Password000!")
            }
        );
        db.SaveChanges();

        db.Tickets.AddRange(
            new Ticket { Id = 1, Title = "Drucker reagiert nicht", Description = "3. OG, Raum 312 – Drucker druckt nicht mehr. Fehlermeldung: Paper Jam.", Priority = Priority.High, Status = TicketStatus.Open, CreatedByUserId = 2 },
            new Ticket { Id = 2, Title = "VPN-Zugang einrichten", Description = "Neuer Mitarbeiter Tom Berger braucht VPN-Zugang für Homeoffice.", Priority = Priority.Medium, Status = TicketStatus.InProgress, CreatedByUserId = 3, AssignedToUserId = 1 },
            new Ticket { Id = 3, Title = "Laptop überhitzt", Description = "Notebook wird sehr heiß, Lüfter sehr laut. Möglicher Hardware-Defekt.", Priority = Priority.Critical, Status = TicketStatus.Open, CreatedByUserId = 2 },
            new Ticket { Id = 4, Title = "Software-Update fehlgeschlagen", Description = "Windows Update bricht mit Fehlercode 0x80070057 ab.", Priority = Priority.Low, Status = TicketStatus.Closed, CreatedByUserId = 3, AssignedToUserId = 1 },
            new Ticket { Id = 5, Title = "Monitor zeigt kein Bild", Description = "Bildschirm bleibt nach Neustart schwarz. Kabel bereits geprüft.", Priority = Priority.High, Status = TicketStatus.Open, CreatedByUserId = 4 },
            new Ticket { Id = 6, Title = "E-Mail-Konto nicht erreichbar", Description = "Outlook verbindet sich nicht mit dem Exchange-Server.", Priority = Priority.Medium, Status = TicketStatus.InProgress, CreatedByUserId = 2, AssignedToUserId = 1 },
            new Ticket { Id = 7, Title = "Neue Maus anfordern", Description = "Maus-Scrollrad funktioniert nicht mehr.", Priority = Priority.Low, Status = TicketStatus.Open, CreatedByUserId = 4 }
        );
        db.SaveChanges();

        // Demo-Kommentare
        db.Comments.AddRange(
            new Comment { TicketId = 2, Text = "VPN-Client wurde installiert, warte auf Freigabe vom Netzwerk-Team.", AuthorId = 1, CreatedAt = DateTime.UtcNow.AddHours(-5) },
            new Comment { TicketId = 2, Text = "Freigabe erteilt, bitte nochmals testen.", AuthorId = 1, CreatedAt = DateTime.UtcNow.AddHours(-2) },
            new Comment { TicketId = 4, Text = "Update manuell durchgeführt, Problem behoben.", AuthorId = 1, CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new Comment { TicketId = 6, Text = "Exchange-Server Passwort zurückgesetzt, bitte neu einloggen.", AuthorId = 1, CreatedAt = DateTime.UtcNow.AddHours(-1) }
        );
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HelpDesk Pro v1"));
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
