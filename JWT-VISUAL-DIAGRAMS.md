# 🎨 JWT Authentication - Visual Flow Diagrams

## 📐 Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────┐
│                        INDIATRAILS API ARCHITECTURE                 │
│                                                                     │
│  ┌──────────────┐       ┌──────────────┐       ┌──────────────┐   │
│  │   Flutter    │       │   ASP.NET    │       │  SQL Server  │   │
│  │   Mobile     │◄─────►│   Core API   │◄─────►│   Database   │   │
│  │     App      │  JWT  │              │       │              │   │
│  └──────────────┘       └──────────────┘       └──────────────┘   │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 🔄 Complete Authentication Flow

```
┌─────────────────────────────────────────────────────────────────────────┐
│                     REGISTRATION FLOW                                   │
└─────────────────────────────────────────────────────────────────────────┘

    Flutter App                  API Server              Database
        │                            │                       │
        │  POST /api/auth/register   │                       │
        │  {username, email, pwd}    │                       │
        ├───────────────────────────►│                       │
        │                            │                       │
        │                            │  Check if user exists │
        │                            ├──────────────────────►│
        │                            │                       │
        │                            │◄──────────────────────┤
        │                            │  User not found       │
        │                            │                       │
        │                            │  Hash password        │
        │                            │  (BCrypt)             │
        │                            │                       │
        │                            │  INSERT INTO Users    │
        │                            ├──────────────────────►│
        │                            │                       │
        │                            │◄──────────────────────┤
        │                            │  User created         │
        │                            │                       │
        │                            │  Generate JWT         │
        │                            │  Token                │
        │                            │                       │
        │  200 OK                    │                       │
        │  {token, username, email}  │                       │
        │◄───────────────────────────┤                       │
        │                            │                       │
        │  Save token to             │                       │
        │  SharedPreferences         │                       │
        │                            │                       │


┌─────────────────────────────────────────────────────────────────────────┐
│                         LOGIN FLOW                                      │
└─────────────────────────────────────────────────────────────────────────┘

    Flutter App                  API Server              Database
        │                            │                       │
        │  POST /api/auth/login      │                       │
        │  {email, password}         │                       │
        ├───────────────────────────►│                       │
        │                            │                       │
        │                            │  SELECT * FROM Users  │
        │                            │  WHERE email = ?      │
        │                            ├──────────────────────►│
        │                            │                       │
        │                            │◄──────────────────────┤
        │                            │  User data            │
        │                            │                       │
        │                            │  Verify password      │
        │                            │  BCrypt.Verify()      │
        │                            │                       │
        │                            │  ✓ Password valid     │
        │                            │                       │
        │                            │  UPDATE LastLoginAt   │
        │                            ├──────────────────────►│
        │                            │                       │
        │                            │  Generate JWT Token   │
        │                            │                       │
        │  200 OK                    │                       │
        │  {token, username, email}  │                       │
        │◄───────────────────────────┤                       │
        │                            │                       │
        │  Save token                │                       │
        │                            │                       │


┌─────────────────────────────────────────────────────────────────────────┐
│                    PROTECTED REQUEST FLOW                               │
└─────────────────────────────────────────────────────────────────────────┘

    Flutter App                  API Server              Database
        │                            │                       │
        │  POST /api/region          │                       │
        │  Header:                   │                       │
        │  Authorization: Bearer... │                       │
        │  Body: {region data}       │                       │
        ├───────────────────────────►│                       │
        │                            │                       │
        │                            │  Validate JWT Token   │
        │                            │  ├─ Check signature   │
        │                            │  ├─ Check expiry      │
        │                            │  ├─ Check issuer      │
        │                            │  └─ Check audience    │
        │                            │                       │
        │                            │  ✓ Token valid        │
        │                            │                       │
        │                            │  Extract user claims  │
        │                            │  (userId, email)      │
        │                            │                       │
        │                            │  INSERT INTO Regions  │
        │                            ├──────────────────────►│
        │                            │                       │
        │                            │◄──────────────────────┤
        │                            │  Region created       │
        │                            │                       │
        │  201 Created               │                       │
        │  {region data}             │                       │
        │◄───────────────────────────┤                       │
        │                            │                       │


┌─────────────────────────────────────────────────────────────────────────┐
│                    FAILED AUTHENTICATION FLOW                           │
└─────────────────────────────────────────────────────────────────────────┘

    Flutter App                  API Server              Database
        │                            │                       │
        │  POST /api/region          │                       │
        │  (NO Authorization Header) │                       │
        ├───────────────────────────►│                       │
        │                            │                       │
        │                            │  Check for JWT Token  │
        │                            │                       │
        │                            │  ✗ No token found     │
        │                            │                       │
        │  401 Unauthorized          │                       │
        │◄───────────────────────────┤                       │
        │                            │                       │
        │  Show error message        │                       │
        │  "Please login"            │                       │
        │                            │                       │
```

---

## 🔐 JWT Token Structure

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        JWT TOKEN STRUCTURE                              │
└─────────────────────────────────────────────────────────────────────────┘

    HEADER.PAYLOAD.SIGNATURE

┌──────────────────────────────────────────────────────────────┐
│                          HEADER                              │
├──────────────────────────────────────────────────────────────┤
│  {                                                           │
│    "alg": "HS256",      ◄── Algorithm                        │
│    "typ": "JWT"         ◄── Type                             │
│  }                                                           │
└──────────────────────────────────────────────────────────────┘
                            │
                            │ Base64Url encoded
                            ▼
         eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9

┌──────────────────────────────────────────────────────────────┐
│                         PAYLOAD                              │
├──────────────────────────────────────────────────────────────┤
│  {                                                           │
│    "nameid": "user-guid",        ◄── User ID                 │
│    "unique_name": "pradeep",     ◄── Username                │
│    "email": "p@aurigo.com",      ◄── Email                   │
│    "sub": "p@aurigo.com",        ◄── Subject                 │
│    "jti": "token-guid",          ◄── Token ID                │
│    "exp": 1732104000,            ◄── Expiration              │
│    "iss": "https://localhost",   ◄── Issuer                  │
│    "aud": "https://localhost"    ◄── Audience                │
│  }                                                           │
└──────────────────────────────────────────────────────────────┘
                            │
                            │ Base64Url encoded
                            ▼
         eyJuYW1laWQiOiJ1c2VyLWd1aWQiLC...

┌──────────────────────────────────────────────────────────────┐
│                        SIGNATURE                             │
├──────────────────────────────────────────────────────────────┤
│  HMACSHA256(                                                 │
│    base64UrlEncode(header) + "." +                           │
│    base64UrlEncode(payload),                                 │
│    secret-key                                                │
│  )                                                           │
└──────────────────────────────────────────────────────────────┘
                            │
                            ▼
         SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
```

---

## 🏗️ Project Structure

```
IndiaTrails.API/
│
├── 📁 Controllers/
│   ├── 📄 AuthController.cs          ◄── Login/Register endpoints
│   ├── 📄 RegionController.cs        ◄── Protected by [Authorize]
│   └── 📄 WalksController.cs         ◄── Protected by [Authorize]
│
├── 📁 Models/
│   ├── 📁 Domain/
│   │   ├── 📄 User.cs                ◄── User entity
│   │   ├── 📄 Region.cs              ◄── Region entity
│   │   └── 📄 Walk.cs                ◄── Walk entity
│   │
│   └── 📁 DTOs/
│       ├── 📁 Request/
│       │   ├── 📄 RegisterRequestDto.cs
│       │   ├── 📄 LoginRequestDto.cs
│       │   ├── 📄 AddRegionRequestDto.cs
│       │   └── 📄 AddWalkRequestDto.cs
│       │
│       └── 📁 Response/
│           ├── 📄 AuthResponseDto.cs
│           ├── 📄 RegionResponseDto.cs
│           └── 📄 WalkResponseDto.cs
│
├── 📁 Repositories/
│   ├── 📁 Contracts/
│   │   ├── 📄 IAuthRepository.cs     ◄── Auth interface
│   │   ├── 📄 IRegionRepository.cs
│   │   └── 📄 IWalkRepository.cs
│   │
│   ├── 📄 SqlServerAuthRepository.cs ◄── JWT generation here
│   ├── 📄 SqlServerRegionRepository.cs
│   └── 📄 SqlServerWalkRepository.cs
│
├── 📁 Data/
│   └── 📄 IndiaTrailsDBContext.cs    ◄── DbContext with Users
│
├── 📁 Migrations/
│   └── 📄 ..._AddUserAuthentication.cs
│
├── 📄 Program.cs                     ◄── JWT configuration
├── 📄 appsettings.json               ◄── JWT settings
└── 📄 IndiaTrails.API.csproj         ◄── NuGet packages
```

---

## 🔄 Authentication Middleware Pipeline

```
┌─────────────────────────────────────────────────────────────────┐
│                    REQUEST PIPELINE                             │
└─────────────────────────────────────────────────────────────────┘

    HTTP Request
        │
        ▼
    ┌─────────────────────┐
    │   HTTPS Redirect    │
    └──────────┬──────────┘
               │
               ▼
    ┌─────────────────────┐
    │  UseAuthentication  │  ◄── Validate JWT Token
    │                     │      • Check signature
    │                     │      • Check expiry
    │                     │      • Extract claims
    └──────────┬──────────┘
               │
               ├─── Token Valid ───┐
               │                   │
               └─── Token Invalid ─┤
                                   │
                                   ▼
                        ┌─────────────────────┐
                        │  UseAuthorization   │  ◄── Check [Authorize]
                        │                     │      attribute
                        └──────────┬──────────┘
                                   │
                                   ├─── Authorized ───┐
                                   │                  │
                                   └─── Not Authorized┤
                                                      │
                                                      ▼
                                            ┌─────────────────────┐
                                            │   Controller        │
                                            │   Action            │
                                            └─────────────────────┘
```

---

## 📊 Database Schema

```
┌─────────────────────────────────────────────────────────────────┐
│                         DATABASE SCHEMA                         │
└─────────────────────────────────────────────────────────────────┘

┌────────────────────────┐
│       Users            │
├────────────────────────┤
│ 🔑 Id (PK, GUID)       │
│ 📧 Username            │
│ 📧 Email (Unique)      │
│ 🔒 PasswordHash        │
│ 📅 CreatedAt           │
│ 📅 LastLoginAt         │
└────────────────────────┘

┌────────────────────────┐          ┌────────────────────────┐
│       Regions          │          │        Walks           │
├────────────────────────┤          ├────────────────────────┤
│ 🔑 Id (PK)             │          │ 🔑 Id (PK)             │
│ 📝 Name                │◄─────────┤ 🔗 RegionId (FK)       │
│ 📝 Code                │    1:N   │ 📝 Name                │
│ 🖼️  RegionImageUrl     │          │ 📝 Description         │
└────────────────────────┘          │ 📏 LengthInKm          │
                                    │ 🔗 DifficultyId (FK)   │
                                    │ 🖼️  WalkImageUrl       │
                                    └───────────┬────────────┘
                                                │
                                                │ N:1
                                                │
                                    ┌───────────▼────────────┐
                                    │     Difficulties       │
                                    ├────────────────────────┤
                                    │ 🔑 Id (PK)             │
                                    │ 📝 Name                │
                                    └────────────────────────┘
```

---

## 🎯 Authorization Patterns Comparison

```
┌─────────────────────────────────────────────────────────────────┐
│              PATTERN 1: ALL PROTECTED                           │
└─────────────────────────────────────────────────────────────────┘

    [Authorize]
    public class RegionController
    {
        GET    /api/region          🔒 Protected
        GET    /api/region/{id}     🔒 Protected
        POST   /api/region          🔒 Protected
        PUT    /api/region/{id}     🔒 Protected
        DELETE /api/region/{id}     🔒 Protected
    }

    Use Case: Admin-only APIs, Internal tools


┌─────────────────────────────────────────────────────────────────┐
│              PATTERN 2: MIXED (RECOMMENDED)                     │
└─────────────────────────────────────────────────────────────────┘

    [Authorize]
    public class RegionController
    {
        [AllowAnonymous]
        GET    /api/region          🌐 Public
        
        [AllowAnonymous]
        GET    /api/region/{id}     🌐 Public
        
        POST   /api/region          🔒 Protected
        PUT    /api/region/{id}     🔒 Protected
        DELETE /api/region/{id}     🔒 Protected
    }

    Use Case: Public read, authenticated write (Most common)


┌─────────────────────────────────────────────────────────────────┐
│              PATTERN 3: SELECTIVE                               │
└─────────────────────────────────────────────────────────────────┘

    public class RegionController
    {
        GET    /api/region          🌐 Public
        GET    /api/region/{id}     🌐 Public
        
        [Authorize]
        POST   /api/region          🔒 Protected
        
        [Authorize]
        PUT    /api/region/{id}     🔒 Protected
        
        [Authorize]
        DELETE /api/region/{id}     🔒 Protected
    }

    Use Case: Mostly public with few protected endpoints
```

---

## 📱 Flutter App Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                    FLUTTER APP FLOW                             │
└─────────────────────────────────────────────────────────────────┘

    App Start
        │
        ▼
    ┌─────────────────┐
    │  Check Token    │
    │  in Storage     │
    └────────┬────────┘
             │
      ┌──────┴──────┐
      │             │
      ▼             ▼
  No Token      Has Token
      │             │
      │             ▼
      │      ┌─────────────────┐
      │      │  Validate Token │
      │      │  (Check expiry) │
      │      └────────┬────────┘
      │               │
      │         ┌─────┴─────┐
      │         │           │
      │       Valid      Expired
      │         │           │
      ▼         ▼           │
  ┌──────────────────┐     │
  │  Login Screen    │◄────┘
  └────────┬─────────┘
           │
           │ Login Success
           ▼
       Save Token
           │
           ▼
  ┌──────────────────┐
  │   Home Screen    │
  └────────┬─────────┘
           │
           │ Protected Request
           ▼
    Add Authorization
    Header with Token
           │
           ▼
    ┌─────────────────┐
    │   API Request   │
    └────────┬────────┘
             │
      ┌──────┴──────┐
      │             │
      ▼             ▼
  Success       401 Error
      │             │
      │             ▼
      │      Clear Token
      │             │
      │             ▼
      │      Go to Login
      │
      ▼
  Show Result
```

---

## 🔍 Token Validation Process

```
┌─────────────────────────────────────────────────────────────────┐
│                    TOKEN VALIDATION                             │
└─────────────────────────────────────────────────────────────────┘

Request arrives with token
        │
        ▼
    Extract token from
    Authorization header
        │
        ▼
    ┌─────────────────────┐
    │  Decode JWT Header  │
    └──────────┬──────────┘
               │
               ▼
    Check algorithm = HS256
               │
               ├─── ✗ Wrong algorithm ─────► 401 Unauthorized
               │
               ▼ ✓
    ┌─────────────────────┐
    │  Decode JWT Payload │
    └──────────┬──────────┘
               │
               ▼
    Check expiry (exp claim)
               │
               ├─── ✗ Expired ─────────────► 401 Unauthorized
               │
               ▼ ✓
    Check issuer (iss claim)
               │
               ├─── ✗ Wrong issuer ─────────► 401 Unauthorized
               │
               ▼ ✓
    Check audience (aud claim)
               │
               ├─── ✗ Wrong audience ───────► 401 Unauthorized
               │
               ▼ ✓
    ┌─────────────────────┐
    │  Verify Signature   │
    │  Using secret key   │
    └──────────┬──────────┘
               │
               ├─── ✗ Invalid signature ────► 401 Unauthorized
               │
               ▼ ✓
    ┌─────────────────────┐
    │  Extract Claims     │
    │  (nameid, email)    │
    └──────────┬──────────┘
               │
               ▼
    Populate User.Identity
               │
               ▼
    Continue to controller
```

---

## 🎨 Visual Response Codes

```
┌─────────────────────────────────────────────────────────────────┐
│                    HTTP STATUS CODES                            │
└─────────────────────────────────────────────────────────────────┘

✅ 200 OK
   └── Login successful, Registration successful

✅ 201 Created
   └── Resource created (Region, Walk)

⚠️  400 Bad Request
   └── Invalid data, User already exists, Validation failed

🔒 401 Unauthorized
   └── No token, Invalid token, Expired token, Wrong credentials

🚫 403 Forbidden
   └── Valid token but insufficient permissions (future use)

❌ 404 Not Found
   └── Resource not found

❌ 500 Internal Server Error
   └── Server error, Database error
```

---

## 📈 Performance & Security

```
┌─────────────────────────────────────────────────────────────────┐
│                 SECURITY BEST PRACTICES                         │
└─────────────────────────────────────────────────────────────────┘

    ✅ Password Hashing
       └── BCrypt with salt rounds

    ✅ JWT Expiration
       └── 7 days (configurable)

    ✅ HTTPS Only
       └── Encrypted communication

    ✅ Token Signature
       └── HMAC-SHA256

    ✅ Input Validation
       └── FluentValidation

    ⚠️  TODO for Production:
       ├── Rate limiting
       ├── Refresh tokens
       ├── Password complexity rules
       ├── Email verification
       ├── Account lockout
       └── Audit logging
```

---

**Created for:** IndiaTrails API Project  
**Developer:** Pradeep Kumar Verma  
**Framework:** .NET 8.0 + Flutter  
**Last Updated:** October 2025
