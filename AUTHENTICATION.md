# 🔐 Authentifizierung in HelpDesk Pro

## Überblick

Die HelpDesk Pro Anwendung implementiert echte Benutzer-Authentifizierung basierend auf:
- **BCrypt** für sichere Passwort-Hashing
- **JWT (JSON Web Tokens)** für zustandslose Authentifizierung
- **Authorization Header** für API-Anfragen

## Demo-Accounts

| Email | Passwort | Rolle |
|-------|----------|-------|
| anna@firma.at | Password123! | Admin |
| max@firma.at | Password456! | Employee |
| lisa@firma.at | Password789! | Employee |
| tom@firma.at | Password000! | Employee |

## Backend (HelpDeskPro.Api)

### User Model mit PasswordHash

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }  // BCrypt gehashtes Passwort
    public UserRole Role { get; set; }
    // ...
}
```

### Authentication Endpoints

#### `POST /api/auth/login`

**Request:**
```json
{
  "email": "anna@firma.at",
  "password": "Password123!"
}
```

**Success Response (200 OK):**
```json
{
  "userId": 1,
  "name": "Admin Anna",
  "email": "anna@firma.at",
  "role": "Admin",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Error Response (401 Unauthorized):**
```json
"Ungültige E-Mail oder Passwort."
```

### JWT Token

- **Signatur:** HS256 (HMAC mit SHA-256)
- **Secret:** Aus `appsettings.json` (Feld: `JwtSecret`)
- **Claims:**
  - `NameIdentifier`: User ID
  - `Email`: User E-Mail
  - `Name`: User Name
  - `Role`: User Rolle
- **Gültigkeit:** 1440 Minuten (24 Stunden) standardmäßig
- **Issuer:** "HelpDeskPro"
- **Audience:** "HelpDeskProClient"

### API Authentifizierung

Alle Endpoints außer `/api/auth/login` erfordern ein gültiges JWT Token im Authorization Header:

```
Authorization: Bearer <JWT_TOKEN>
```

**Middleware konfiguriert in `Program.cs`:**
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* ... */ });
```

### Controller-Level Security

```csharp
[ApiController]
[Route("api/tickets")]
[Authorize]  // Erfordert authentifizierten Benutzer
public class TicketsController : ControllerBase
```

## Frontend (HelpDeskPro.Wpf)

### LoginWindow

**Alte Version (Demo):**
- DropDown mit Benutzerliste
- Beliebiges Passwort akzeptiert

**Neue Version (Echte Authentifizierung):**
- Email-Eingabefeld
- Passwort-Eingabefeld
- Validierung gegen Backend

### AuthState Service

```csharp
public class AuthState
{
    public UserDto? CurrentUser { get; private set; }
    public string? AuthToken { get; private set; }
    public bool IsLoggedIn => CurrentUser is not null && AuthToken is not null;

    public void Login(UserDto user, string token) { /* ... */ }
    public void Logout() { /* ... */ }
}
```

### API Service

- **Token Storage:** Automatisch in `AuthState.AuthToken` gespeichert
- **Token Injection:** Jede API-Anfrage erhält automatisch den Token:
  ```csharp
  _http.DefaultRequestHeaders.Authorization = 
      new AuthenticationHeaderValue("Bearer", _auth.AuthToken);
  ```
- **Login Method:**
  ```csharp
  public async Task<LoginResponse?> LoginAsync(string email, string password)
  ```

## Sicherheits-Services

### PasswordService (HelpDeskPro.Core)

```csharp
public static string HashPassword(string password)
{
    // BCrypt mit Work Factor 12
    return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
}

public static bool VerifyPassword(string password, string hash)
{
    // Zeitkonstanter Vergleich
    return BCrypt.Net.BCrypt.Verify(password, hash);
}
```

**Security Features:**
- **Automatisches Salting:** BCrypt generiert & speichert Salt im Hash
- **Work Factor 12:** Erhöhter CPU-Aufwand gegen Brute-Force
- **Zeitkonstanter Vergleich:** Kein Timing-Attack möglich

### JwtService (HelpDeskPro.Core)

```csharp
public string GenerateToken(User user)
{
    // Token mit Benutzerdaten und Ablauf
    // Signature mit SymmetricSecurityKey
}

public ClaimsPrincipal? ValidateToken(string token)
{
    // Validiert Signatur, Issuer, Audience, Lifetime
}
```

## Datenbank-Setup

### Migrationen

1. **User Model erweitert:**
   - `PasswordHash` Feld hinzugefügt
   - Länge: 255 Zeichen (BCrypt Output: ~60 Zeichen)

2. **Seeding mit gehashten Passwörtern:**
   ```csharp
   new User 
   { 
       Id = 1, 
       Name = "Admin Anna",
       Email = "anna@firma.at",
       PasswordHash = PasswordService.HashPassword("Password123!")
   }
   ```

3. **Datenbank zurückgesetzt:**
   - Alte `helpdeskpro.db` gelöscht
   - Beim nächsten Start neu erstellt mit gehashten Passwörtern

## Best Practices implementiert

| Best Practice | Implementierung |
|---|---|
| **Passwörter hashen** | BCrypt mit Work Factor 12 |
| **Keine Klartextpasswörter** | Nur PasswordHash in DB |
| **Zeitkonstanter Vergleich** | BCrypt.Verify (kein Timing-Attack) |
| **JWT statt Sessions** | Zustandslos, skalierbar |
| **Token Expiration** | 24 Stunden (konfigurierbar) |
| **Authorization Header** | Standard Bearer Token |
| **HTTPS empfohlen** | Derzeit HTTP dev, muss zu HTTPS in Production |
| **CORS konfiguriert** | Erlaubt WPF Client |
| **Generische Error Messages** | "Ungültige E-Mail oder Passwort" (keine Preisgabe) |

## Konfiguration

### appsettings.json

```json
{
  "JwtSecret": "HelpDeskPro-SecretKey-MinimumLength32Characters!",
  "ConnectionStrings": {
    "HelpDeskPro": "Data Source=helpdeskpro.db"
  }
}
```

**Wichtig für Production:**
- `JwtSecret`: Mindestens 32 Zeichen, zufällig generieren
- Environment-spezifische Secrets nutzen
- Nie in Git committen

## Testen

### Swagger UI

```
GET http://localhost:5000/swagger/ui/index.html
```

Endpoints erfordern JWT Token im Authorization Button.

### Manual Testing

```powershell
# 1. Login
$loginResp = Invoke-WebRequest -Uri "http://localhost:5000/api/auth/login" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"email":"anna@firma.at","password":"Password123!"}'

$token = ($loginResp.Content | ConvertFrom-Json).token

# 2. Protected Endpoint mit Token
$headers = @{
  Authorization = "Bearer $token"
}

Invoke-WebRequest -Uri "http://localhost:5000/api/tickets" `
  -Headers $headers
```

## Migration zu Production

1. **JWT Secret:** Aus Environment Variable oder Key Vault laden
2. **HTTPS:** Nur verschlüsselte Verbindungen erlauben
3. **Password Policy:** Mindestlänge, Komplexität, Historie
4. **Rate Limiting:** Login-Versuche begrenzen
5. **Audit Logging:** Failed Logins protokollieren
6. **Refresh Tokens:** Optional für längere Sessions
7. **MFA:** Optional für Admin-Accounts

## Weitere Ressourcen

- [BCrypt.Net-Next](https://github.com/BcryptNet/bcrypt.net)
- [JWT Introduction](https://tools.ietf.org/html/rfc7519)
- [ASP.NET Core Security](https://docs.microsoft.com/en-us/aspnet/core/security/)
- [OWASP Authentication Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html)
