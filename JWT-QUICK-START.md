# ⚡ JWT Implementation - Quick Start Checklist

## 📋 PRE-FLIGHT CHECK
```
□ .NET 8.0 SDK installed
□ SQL Server running
□ Visual Studio / VS Code open
□ Terminal ready
```

---

## 🚀 IMPLEMENTATION STEPS

### 1️⃣ RESTORE & BUILD
```bash
cd D:\Learning\DOTNET\MicroService\IndiaTrails\IndiaTrails.API\IndiaTrails.API
dotnet restore
dotnet build
```
**✅ Expected:** Build succeeded. 0 Warning(s). 0 Error(s).

---

### 2️⃣ CREATE MIGRATION
```bash
dotnet ef migrations add AddUserAuthentication
```
**✅ Expected:** Done. Migration files created in Migrations/ folder

---

### 3️⃣ UPDATE DATABASE
```bash
dotnet ef database update
```
**✅ Expected:** Applying migration '..._AddUserAuthentication'. Done.

**Verify in SSMS:**
```sql
USE IndiaTrailsDB;
SELECT * FROM Users; -- Should exist (empty table)
```

---

### 4️⃣ RUN APPLICATION
```bash
dotnet run
```
**✅ Expected:** Now listening on: https://localhost:7000

---

## 🧪 TESTING SEQUENCE

### Test 1: Register User ✉️
```
POST https://localhost:7000/api/auth/register

Body:
{
  "username": "test_user",
  "email": "test@example.com",
  "password": "test123"
}

Expected: 200 OK + JWT token
```

### Test 2: Login User 🔑
```
POST https://localhost:7000/api/auth/login

Body:
{
  "email": "test@example.com",
  "password": "test123"
}

Expected: 200 OK + JWT token
```

### Test 3: Public Endpoint 🌐
```
GET https://localhost:7000/api/region

Headers: (none needed)

Expected: 200 OK + list of regions
```

### Test 4: Protected WITHOUT Token ❌
```
POST https://localhost:7000/api/region

Headers: (no auth)
Body: { "name": "Test", "code": "TS", "regionImageUrl": "..." }

Expected: 401 Unauthorized
```

### Test 5: Protected WITH Token ✅
```
POST https://localhost:7000/api/region

Headers: 
  Authorization: Bearer {your-token-here}
  
Body: { "name": "Test", "code": "TS", "regionImageUrl": "..." }

Expected: 201 Created
```

---

## 📝 POSTMAN SETUP

### Step 1: Create Environment
```
Environment: IndiaTrails
Variables:
  - baseUrl: https://localhost:7000
  - authToken: (leave empty)
```

### Step 2: Login Request - Tests Tab
```javascript
// Auto-save token after login
if (pm.response.code === 200) {
    pm.environment.set("authToken", pm.response.json().token);
}
```

### Step 3: Protected Requests
```
Authorization Tab:
  Type: Bearer Token
  Token: {{authToken}}
```

---

## 🎯 PROTECTION PATTERNS

### Pattern A: All Protected
```csharp
[Authorize]
public class MyController : ControllerBase
{
    // All methods require auth
}
```

### Pattern B: Mixed (Recommended)
```csharp
[Authorize] // Default
public class MyController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous] // Public
    public IActionResult Get() { }
    
    [HttpPost] // Protected
    public IActionResult Create() { }
}
```

---

## 🐛 QUICK TROUBLESHOOTING

| Error | Fix |
|-------|-----|
| JwtBearerDefaults not found | `dotnet restore` |
| 401 with valid token | Check UseAuthentication() order |
| Token validation failed | Verify JWT Key in appsettings.json |
| DB connection failed | Check SQL Server service |
| BCrypt error | `dotnet clean && dotnet restore` |

---

## 🔍 VERIFY JWT TOKEN
1. Copy token from login response
2. Go to https://jwt.io
3. Paste and decode
4. Verify claims: nameid, unique_name, email, exp

---

## 📱 FLUTTER INTEGRATION

### Add Dependencies
```yaml
dependencies:
  http: ^1.1.0
  shared_preferences: ^2.2.2
```

### Login Example
```dart
final response = await http.post(
  Uri.parse('https://localhost:7000/api/auth/login'),
  headers: {'Content-Type': 'application/json'},
  body: jsonEncode({
    'email': 'user@example.com',
    'password': 'password123',
  }),
);

final token = jsonDecode(response.body)['token'];
// Save token to SharedPreferences
```

### Authenticated Request
```dart
final token = await getStoredToken();
final response = await http.post(
  Uri.parse('https://localhost:7000/api/region'),
  headers: {
    'Content-Type': 'application/json',
    'Authorization': 'Bearer $token',
  },
  body: jsonEncode({...}),
);
```

---

## ✅ COMPLETION CHECKLIST

```
□ Packages restored successfully
□ Project builds without errors
□ Migration created and applied
□ Users table exists in database
□ Application runs on https://localhost:7000
□ Registration endpoint works (200 OK)
□ Login endpoint works (200 OK)
□ JWT token can be decoded at jwt.io
□ Public endpoints accessible without token
□ Protected endpoints return 401 without token
□ Protected endpoints work WITH token (201/200)
□ Token saved in Postman environment
□ Flutter service created (if applicable)
```

---

## 📊 API ENDPOINTS SUMMARY

| Endpoint | Method | Auth | Purpose |
|----------|--------|------|---------|
| /api/auth/register | POST | No | Register user |
| /api/auth/login | POST | No | Get JWT token |
| /api/region | GET | No | List regions |
| /api/region/{id} | GET | No | Get region |
| /api/region | POST | Yes | Create region |
| /api/region/{id} | PUT | Yes | Update region |
| /api/region/{id} | DELETE | Yes | Delete region |
| /api/walks | GET | No | List walks |
| /api/walks/{id} | GET | No | Get walk |
| /api/walks | POST | Yes | Create walk |
| /api/walks/{id} | PUT | Yes | Update walk |
| /api/walks/{id} | DELETE | Yes | Delete walk |

---

## 🔐 SECURITY NOTES

**⚠️ Before Production:**
- Change JWT secret key
- Use environment variables
- Enable HTTPS only
- Add rate limiting
- Implement refresh tokens
- Add logging
- Set up monitoring

---

## 📞 SUPPORT

**Documentation:**
- Full Guide: JWT-VISUAL-GUIDE.md
- Implementation: JWT-IMPLEMENTATION-GUIDE.md
- Quick Reference: JWT-QUICK-REFERENCE.md

**Useful Links:**
- JWT Decoder: https://jwt.io
- BCrypt Test: https://bcrypt-generator.com
- Swagger UI: https://localhost:7000/swagger

---

**Project:** IndiaTrails API  
**Developer:** Pradeep Kumar Verma  
**Email:** pradeep@aurigo.com  
**Last Updated:** October 2025

---

## 💡 TIPS

1. **Save Token**: Always copy token after login for testing
2. **Check Expiry**: Tokens expire in 7 days by default
3. **Use Postman**: Save time with environment variables
4. **Test Public First**: Verify public endpoints before testing auth
5. **Check Order**: UseAuthentication() must come before UseAuthorization()
6. **Decode Tokens**: Use jwt.io to inspect token contents
7. **SQL Check**: Verify Users table exists: `SELECT * FROM Users`
8. **Clean Build**: If errors persist, run `dotnet clean && dotnet restore`

---

**✨ You're ready to implement JWT authentication! ✨**
