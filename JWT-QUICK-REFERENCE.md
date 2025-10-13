# JWT Quick Reference Card

## 🚀 Quick Start Commands

### Database Migration
```bash
# Navigate to project directory
cd D:\Learning\DOTNET\MicroService\IndiaTrails\IndiaTrails.API\IndiaTrails.API

# Add migration
dotnet ef migrations add AddUserAuthentication

# Update database
dotnet ef database update

# Restore packages
dotnet restore

# Run the application
dotnet run
```

## 📝 API Endpoints

### Authentication Endpoints

#### Register User
```http
POST https://localhost:7000/api/auth/register
Content-Type: application/json

{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "password123"
}
```

#### Login User
```http
POST https://localhost:7000/api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "password123"
}
```

### Protected Endpoint Example
```http
POST https://localhost:7000/api/region
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "name": "Rajasthan",
  "code": "RJ",
  "regionImageUrl": "https://example.com/image.jpg"
}
```

## 🔒 Controller Protection Patterns

### Pattern 1: Protect Entire Controller
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class YourController : ControllerBase
{
    // All methods require authentication
}
```

### Pattern 2: Protect Specific Methods
```csharp
[ApiController]
[Route("api/[controller]")]
public class YourController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(); // Public
    
    [HttpPost]
    [Authorize] // Protected
    public IActionResult Create() => Ok();
}
```

### Pattern 3: Mixed (Recommended)
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // Default: Protected
public class YourController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous] // Override: Public
    public IActionResult GetAll() => Ok();
    
    [HttpPost] // Protected (inherited)
    public IActionResult Create() => Ok();
}
```

## 🎯 Access User Claims in Controllers

```csharp
using System.Security.Claims;

[HttpGet("profile")]
[Authorize]
public IActionResult GetProfile()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var username = User.FindFirst(ClaimTypes.Name)?.Value;
    var email = User.FindFirst(ClaimTypes.Email)?.Value;
    
    return Ok(new { userId, username, email });
}
```

## 🧪 Testing with cURL

### Register
```bash
curl -X POST https://localhost:7000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "john_doe",
    "email": "john@example.com",
    "password": "password123"
  }'
```

### Login
```bash
curl -X POST https://localhost:7000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "password123"
  }'
```

### Protected Endpoint
```bash
curl -X POST https://localhost:7000/api/region \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Rajasthan",
    "code": "RJ",
    "regionImageUrl": "https://example.com/image.jpg"
  }'
```

## ⚙️ Configuration (appsettings.json)

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong123456",
    "Issuer": "https://localhost:7000",
    "Audience": "https://localhost:7000"
  }
}
```

## 🛠️ Common Issues & Solutions

| Issue | Solution |
|-------|----------|
| 401 Unauthorized with valid token | Check `UseAuthentication()` comes before `UseAuthorization()` in Program.cs |
| Token not recognized | Verify Authorization header format: `Bearer {token}` |
| Migration fails | Check SQL Server is running and connection string is correct |
| BCrypt error | Run `dotnet restore` to ensure BCrypt.Net-Next is installed |

## 📦 Required NuGet Packages

```xml
<PackageReference Include="Microsoft.IdentityModel.Tokens" Version="8.14.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.14.0" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
```

## 🔍 Verify JWT Token

Decode your JWT token at: https://jwt.io

Your token should contain:
- `nameid`: User ID
- `unique_name`: Username
- `email`: User email
- `exp`: Expiration timestamp
- `iss`: Issuer
- `aud`: Audience

## 📊 HTTP Status Codes

- `200 OK` - Successful login/registration with token
- `201 Created` - Resource created successfully
- `400 Bad Request` - Invalid request data / User already exists
- `401 Unauthorized` - Invalid credentials or missing/invalid token
- `404 Not Found` - Resource not found

## 🎨 Postman Setup

1. **Create a new environment**
   - Variable: `baseUrl`, Value: `https://localhost:7000`
   - Variable: `token`, Value: (empty initially)

2. **After login, set token**
   - In Tests tab of login request:
   ```javascript
   pm.environment.set("token", pm.response.json().token);
   ```

3. **Use token in requests**
   - Authorization Type: Bearer Token
   - Token: `{{token}}`

## 🔐 Security Checklist

- [ ] Change JWT secret key in production
- [ ] Use environment variables for sensitive data
- [ ] Enable HTTPS only
- [ ] Implement refresh tokens
- [ ] Add rate limiting
- [ ] Enable CORS properly
- [ ] Add logging for authentication events
- [ ] Implement password strength requirements
- [ ] Add email verification
- [ ] Consider 2FA for sensitive operations

---
**Created:** $(date)
**Project:** IndiaTrails.API
**Framework:** .NET 8.0
