# JWT Authentication Implementation Guide for IndiaTrails API

## Overview
This guide documents the complete implementation of JWT (JSON Web Token) authentication in the IndiaTrails API.

## What Was Implemented

### 1. **Models Created**
- **User.cs**: Domain model for user authentication
  - Properties: Id, Username, Email, PasswordHash, CreatedAt, LastLoginAt

### 2. **DTOs Created**
- **RegisterRequestDto.cs**: For user registration
- **LoginRequestDto.cs**: For user login
- **AuthResponseDto.cs**: Returns JWT token and user info

### 3. **Repository Pattern**
- **IAuthRepository.cs**: Interface defining authentication operations
- **SqlServerAuthRepository.cs**: Implementation with:
  - User registration with password hashing (BCrypt)
  - User login with password verification
  - JWT token generation
  - User existence check

### 4. **Controller**
- **AuthController.cs**: Handles authentication endpoints
  - POST `/api/auth/register` - User registration
  - POST `/api/auth/login` - User login

### 5. **Configuration**
- JWT settings added to `appsettings.json`
- Authentication middleware configured in `Program.cs`
- Authorization services registered

## Database Migration Steps

### Step 1: Add Migration
```bash
cd D:\Learning\DOTNET\MicroService\IndiaTrails\IndiaTrails.API\IndiaTrails.API
dotnet ef migrations add AddUserAuthentication
```

### Step 2: Update Database
```bash
dotnet ef database update
```

This will create the `Users` table in your database with columns:
- Id (uniqueidentifier, PK)
- Username (nvarchar(100))
- Email (nvarchar(100))
- PasswordHash (nvarchar(max))
- CreatedAt (datetime2)
- LastLoginAt (datetime2, nullable)

## Restore NuGet Packages

Since we added BCrypt.Net-Next, restore packages:
```bash
dotnet restore
```

## Testing the Authentication

### 1. Register a New User
**Endpoint:** `POST https://localhost:7000/api/auth/register`

**Request Body:**
```json
{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "password123"
}
```

**Expected Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "john_doe",
  "email": "john@example.com"
}
```

### 2. Login
**Endpoint:** `POST https://localhost:7000/api/auth/login`

**Request Body:**
```json
{
  "email": "john@example.com",
  "password": "password123"
}
```

**Expected Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "john_doe",
  "email": "john@example.com"
}
```

### 3. Test Protected Endpoints

#### Without Token (Should Fail)
**Endpoint:** `POST https://localhost:7000/api/region`

**Expected Response:** `401 Unauthorized`

#### With Token (Should Succeed)
**Endpoint:** `POST https://localhost:7000/api/region`

**Headers:**
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
```

**Request Body:**
```json
{
  "name": "Rajasthan",
  "code": "RJ",
  "regionImageUrl": "https://example.com/image.jpg"
}
```

**Expected Response:** `201 Created`

## Protecting Your Controllers

### Method 1: Protect Entire Controller
Add `[Authorize]` attribute to the controller class:

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints require authentication
public class RegionController : ControllerBase
{
    // All methods here require authentication
}
```

### Method 2: Protect Specific Endpoints
Add `[Authorize]` to individual methods:

```csharp
[HttpPost]
[Authorize] // Only this endpoint requires authentication
public async Task<IActionResult> CreateRegion([FromBody] AddRegionRequestDto region)
{
    // Implementation
}
```

### Method 3: Allow Anonymous Access to Specific Endpoints
When controller has `[Authorize]`, you can allow public access to specific endpoints:

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // Controller-level protection
public class RegionController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous] // This endpoint is public
    public async Task<IActionResult> GetAllRegions()
    {
        // Public access allowed
    }

    [HttpPost] // This requires authentication (inherited from controller)
    public async Task<IActionResult> CreateRegion([FromBody] AddRegionRequestDto region)
    {
        // Requires authentication
    }
}
```

## Example Protection Strategy

For your IndiaTrails API, a common pattern would be:

### Public Endpoints (No Auth Required)
- `GET /api/region` - View all regions
- `GET /api/region/{id}` - View single region
- `GET /api/walks` - View all walks
- `GET /api/walks/{id}` - View single walk
- `POST /api/auth/register` - Register
- `POST /api/auth/login` - Login

### Protected Endpoints (Auth Required)
- `POST /api/region` - Create region
- `PUT /api/region/{id}` - Update region
- `DELETE /api/region/{id}` - Delete region
- `POST /api/walks` - Create walk
- `PUT /api/walks/{id}` - Update walk
- `DELETE /api/walks/{id}` - Delete walk

## Using Postman for Testing

### 1. Create Environment Variables
- `baseUrl`: `https://localhost:7000`
- `token`: (will be set after login)

### 2. Register/Login Request
Use the registration or login endpoint, then:
1. Copy the `token` from the response
2. Set it in your environment variable

### 3. For Protected Requests
In the "Authorization" tab:
- Type: Bearer Token
- Token: `{{token}}` (or paste the actual token)

Or use the Headers tab:
```
Key: Authorization
Value: Bearer {{token}}
```

## JWT Token Information

### Token Contains:
- User ID (NameIdentifier claim)
- Username (Name claim)
- Email (Email claim)
- Token expiration (7 days from issue)

### Accessing User Info in Controllers
```csharp
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

## Security Considerations

### Current Implementation
✅ Password hashing with BCrypt (strong hashing algorithm)
✅ Token expiration (7 days)
✅ Secure token signing with HMAC-SHA256
✅ Email validation
✅ Password minimum length (6 characters)

### Production Recommendations
⚠️ **Before deploying to production:**

1. **Change the JWT Secret Key**
   - Use a strong, randomly generated key
   - Store it in User Secrets (development) or Azure Key Vault (production)
   - Never commit the key to source control

2. **Use Environment Variables**
   ```bash
   # In production, set environment variables
   JWT__KEY=your-production-secret-key
   JWT__ISSUER=https://yourapi.com
   JWT__AUDIENCE=https://yourapi.com
   ```

3. **Enable HTTPS Only**
   - Ensure SSL/TLS is properly configured
   - JWT tokens should only be transmitted over HTTPS

4. **Consider Additional Features**
   - Refresh tokens for long-lived sessions
   - Password strength requirements
   - Email verification
   - Two-factor authentication
   - Rate limiting on login attempts
   - Token revocation mechanism

5. **Update Token Expiration**
   - Consider shorter expiration times (e.g., 1 hour)
   - Implement refresh token pattern for better security

## Configuration Files Modified

1. **appsettings.json** - Added JWT configuration
2. **Program.cs** - Added authentication middleware
3. **IndiaTrails.API.csproj** - Added BCrypt.Net-Next package
4. **IndiaTrailsDBContext.cs** - Added Users DbSet
5. **RegionController.cs** - Example of authorization implementation

## Next Steps

1. ✅ Run database migrations
2. ✅ Test registration endpoint
3. ✅ Test login endpoint
4. ✅ Test protected endpoints
5. ⬜ Add authorization to other controllers (WalksController)
6. ⬜ Consider implementing refresh tokens
7. ⬜ Add email verification
8. ⬜ Implement password reset functionality

## Troubleshooting

### Common Issues

**Issue: 401 Unauthorized even with valid token**
- Check if `UseAuthentication()` is called before `UseAuthorization()` in Program.cs
- Verify token is included in Authorization header: `Bearer {token}`
- Check if JWT settings match in appsettings.json

**Issue: Token validation fails**
- Ensure Issuer and Audience match in token generation and validation
- Verify JWT Key is the same in both places
- Check token hasn't expired

**Issue: Database migration fails**
- Ensure connection string is correct
- Check if SQL Server is running
- Verify EF Core tools are installed: `dotnet tool install --global dotnet-ef`

## Summary

Your IndiaTrails API now has complete JWT authentication support! Users can:
- Register new accounts with secure password storage
- Login and receive JWT tokens
- Access protected endpoints using Bearer tokens
- Continue accessing public endpoints without authentication

The implementation follows Clean Architecture principles and integrates seamlessly with your existing repository pattern.
