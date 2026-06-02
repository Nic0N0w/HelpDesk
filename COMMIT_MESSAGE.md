## Echte Benutzer-Authentifizierung implementiert ✅

### 🔐 Wichtige Änderungen

#### Backend (HelpDeskPro.Api)
- **AuthController** mit `POST /api/auth/login` Endpoint
- JWT Authentication Middleware mit HS256 Signatur
- `[Authorize]` auf allen Endpoints außer Login
- Gehashte Demo-Passwörter im Seeding

#### Core Services (HelpDeskPro.Core)
- **PasswordService**: BCrypt Hashing & Verification
- **JwtService**: Token-Generierung & Validierung
- **User Model**: PasswordHash Field für sichere Speicherung
- **IUserRepository**: GetByEmailAsync() implementiert

#### Frontend (HelpDeskPro.Wpf)
- **LoginWindow**: Email + Passwort Input (kein Dropdown mehr)
- **AuthState**: Token-Speicherung & Verwaltung
- **ApiService**: Automatische Token-Injection in Requests
- Logout-Funktionalität: State clearen + zurück zu Login

### 🎯 Demo-Accounts
```
anna@firma.at → Password123! (Admin)
max@firma.at → Password456! (Employee)
lisa@firma.at → Password789! (Employee)
tom@firma.at → Password000! (Employee)
```

### ✨ Security Best Practices
✅ BCrypt WorkFactor 12 (CPU-intensiv)
✅ Automatisches Salting
✅ Zeitkonstanter Passwort-Vergleich
✅ Keine Klartextpasswörter
✅ JWT mit 24h Ablauf
✅ Generische Error-Messages
✅ [Authorize] auf Controllern

### 📦 Neue Packages
- BCrypt.Net-Next 4.0.3
- System.IdentityModel.Tokens.Jwt 8.2.1
- Microsoft.AspNetCore.Authentication.JwtBearer 10.0.0


### ❌ Removed
- Demo-Login mit Dropdown
- "Beliebiges Passwort akzeptiert" Logik
- Unverschlüsselte Passwörter
- Alte Demo-Hinweise

### 🚀 Testing
```bash
# Backend
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"anna@firma.at","password":"Password123!"}'

# Response: 200 OK + JWT Token
```

Build: ✅ Erfolgreich
All Tests: ✅ Grün
Ready: ✅ Ja
