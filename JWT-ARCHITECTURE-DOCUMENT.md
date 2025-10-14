# JWT Authentication Architecture - IndiaTrails API
## Technical Architecture Document for Stakeholders

---

## 📋 Table of Contents
1. [Executive Summary](#executive-summary)
2. [Architecture Overview](#architecture-overview)
3. [Authentication Flow](#authentication-flow)
4. [System Components](#system-components)
5. [Database Schema](#database-schema)
6. [API Endpoints](#api-endpoints)
7. [Security Implementation](#security-implementation)
8. [Request/Response Flow](#requestresponse-flow)
9. [Technology Stack](#technology-stack)
10. [Deployment Considerations](#deployment-considerations)

---

## 📊 Executive Summary

### What is JWT Authentication?
JWT (JSON Web Token) is an industry-standard method for securely transmitting information between parties as a JSON object. It enables **stateless authentication**, meaning the server doesn't need to store session information.

### Why JWT for IndiaTrails API?
✅ **Stateless** - No server-side session storage required  
✅ **Scalable** - Easy to scale horizontally across multiple servers  
✅ **Mobile-Friendly** - Perfect for Flutter mobile app integration  
✅ **Industry Standard** - Widely adopted and supported  
✅ **Secure** - Cryptographically signed tokens prevent tampering  

### Key Benefits
- **Performance**: No database lookup on every request
- **Flexibility**: Token can contain user claims and roles
- **Cross-Domain**: Works seamlessly across different domains
- **Microservices Ready**: Easy to implement in distributed systems

---

## 🏗️ Architecture Overview

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        Client Applications                       │
│                 (Flutter Mobile App, Web App)                    │
└───────────────────────────────┬─────────────────────────────────┘
                                │
                                │ HTTPS
                                │
┌───────────────────────────────▼─────────────────────────────────┐
│                      IndiaTrails.API                             │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │            Authentication Middleware                        │ │
│  │  - Validates JWT Token                                      │ │
│  │  - Extracts User Claims                                     │ │
│  │  - Authorization Checks                                     │ │
│  └────────────────────────────────────────────────────────────┘ │
│                                                                   │
│  ┌─────────────┐  ┌──────────────┐  ┌──────────────────────┐  │
│  │   Auth      │  │   Region     │  │   Walks              │  │
│  │ Controller  │  │  Controller  │  │  Controller          │  │
│  └─────────────┘  └──────────────┘  └──────────────────────┘  │
│         │                 │                    │                 │
│  ┌──────▼─────────────────▼────────────────────▼──────────┐   │
│  │              Repository Layer                           │   │
│  │  - IAuthRepository                                      │   │
│  │  - IRegionRepository                                    │   │
│  │  - IWalkRepository                                      │   │
│  └─────────────────────────────────────────────────────────┘   │
│         │                 │                    │                 │
│  ┌──────▼─────────────────▼────────────────────▼──────────┐   │
│  │              Entity Framework Core                      │   │
│  └─────────────────────────────────────────────────────────┘   │
└───────────────────────────────┬─────────────────────────────────┘
                                │
┌───────────────────────────────▼─────────────────────────────────┐
│                     SQL Server Database                          │
│  Tables: Users, Regions, Walks, Difficulties                    │
└─────────────────────────────────────────────────────────────────┘
```

### Clean Architecture Layers

```
┌─────────────────────────────────────────────────────────────┐
│  Presentation Layer (Controllers)                           │
│  - AuthController                                           │
│  - RegionController                                         │
│  - WalksController                                          │
└────────────────────────────┬────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────┐
│  Application Layer (DTOs, Validators)                       │
│  - Request/Response DTOs                                    │
│  - FluentValidation Rules                                   │
│  - AutoMapper Profiles                                      │
└────────────────────────────┬────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────┐
│  Domain Layer (Business Logic)                              │
│  - Repository Interfaces                                    │
│  - Domain Models (User, Region, Walk)                       │
└────────────────────────────┬────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────┐
│  Infrastructure Layer (Data Access)                         │
│  - Repository Implementations                               │
│  - DbContext                                                │
│  - Database Migrations                                      │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔐 Authentication Flow

### 1. User Registration Flow

```mermaid
sequenceDiagram
    participant Client as Flutter App
    participant API as IndiaTrails API
    participant Auth as AuthController
    participant Repo as AuthRepository
    participant BCrypt as BCrypt Library
    participant DB as SQL Server

    Client->>API: POST /api/auth/register
    Note over Client,API: { username, email, password }
    
    API->>Auth: RegisterRequestDto
    Auth->>Repo: RegisterAsync(user, password)
    
    Repo->>BCrypt: HashPassword(password)
    BCrypt-->>Repo: hashedPassword
    
    Repo->>DB: INSERT INTO Users
    DB-->>Repo: User Created
    
    Repo->>Repo: GenerateJwtToken(user)
    Repo-->>Auth: User + JWT Token
    
    Auth-->>API: AuthResponseDto
    API-->>Client: 200 OK + Token
    Note over Client,API: { token, username, email }
```

### 2. User Login Flow

```mermaid
sequenceDiagram
    participant Client as Flutter App
    participant API as IndiaTrails API
    participant Auth as AuthController
    participant Repo as AuthRepository
    participant BCrypt as BCrypt Library
    participant DB as SQL Server

    Client->>API: POST /api/auth/login
    Note over Client,API: { email, password }
    
    API->>Auth: LoginRequestDto
    Auth->>Repo: LoginAsync(email, password)
    
    Repo->>DB: SELECT * FROM Users WHERE email
    DB-->>Repo: User Record
    
    Repo->>BCrypt: Verify(password, hashedPassword)
    BCrypt-->>Repo: Valid/Invalid
    
    alt Password Valid
        Repo->>DB: UPDATE LastLoginAt
        Repo->>Repo: GenerateJwtToken(user)
        Repo-->>Auth: User + JWT Token
        Auth-->>API: AuthResponseDto
        API-->>Client: 200 OK + Token
    else Password Invalid
        Repo-->>Auth: null
        Auth-->>API: Unauthorized
        API-->>Client: 401 Unauthorized
    end
```

### 3. Protected Resource Access Flow

```mermaid
sequenceDiagram
    participant Client as Flutter App
    participant API as IndiaTrails API
    participant AuthMiddleware as JWT Middleware
    participant Controller as RegionController
    participant Repo as RegionRepository
    participant DB as SQL Server

    Client->>API: POST /api/region
    Note over Client,API: Authorization: Bearer {token}<br/>{ name, code, imageUrl }
    
    API->>AuthMiddleware: Validate JWT Token
    
    alt Token Valid
        AuthMiddleware->>AuthMiddleware: Extract Claims
        Note over AuthMiddleware: userId, username, email
        AuthMiddleware->>Controller: Authorized Request
        Controller->>Repo: CreateAsync(region)
        Repo->>DB: INSERT INTO Regions
        DB-->>Repo: Region Created
        Repo-->>Controller: Region
        Controller-->>API: 201 Created
        API-->>Client: Region Created
    else Token Invalid/Missing
        AuthMiddleware-->>API: 401 Unauthorized
        API-->>Client: 401 Unauthorized
    end
```

---

## 🧩 System Components

### Component Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                     Controllers Layer                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  AuthController                                           │  │
│  │  ────────────────                                         │  │
│  │  + Register(RegisterRequestDto)                           │  │
│  │  + Login(LoginRequestDto)                                 │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                   │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  RegionController [Authorize]                             │  │
│  │  ──────────────────────────                               │  │
│  │  + GetAllRegions() [AllowAnonymous]                       │  │
│  │  + GetRegionById(id) [AllowAnonymous]                     │  │
│  │  + CreateRegion(dto) [Requires Auth]                      │  │
│  │  + UpdateRegion(id, dto) [Requires Auth]                  │  │
│  │  + DeleteRegion(id) [Requires Auth]                       │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                   Repository Interfaces                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  IAuthRepository                                          │  │
│  │  ────────────────                                         │  │
│  │  + RegisterAsync(user, password)                          │  │
│  │  + LoginAsync(email, password)                            │  │
│  │  + UserExistsAsync(email)                                 │  │
│  │  + GenerateJwtToken(user)                                 │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                   │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  IRegionRepository                                        │  │
│  │  ──────────────────                                       │  │
│  │  + GetAllAsync()                                          │  │
│  │  + GetByIdAsync(id)                                       │  │
│  │  + CreateAsync(region)                                    │  │
│  │  + UpdateAsync(id, region)                                │  │
│  │  + DeleteAsync(id)                                        │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                 Repository Implementations                       │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  SqlServerAuthRepository : IAuthRepository                │  │
│  │  ──────────────────────────────────────                   │  │
│  │  - IndiaTrailsDBContext _context                          │  │
│  │  - IConfiguration _configuration                          │  │
│  │  + RegisterAsync() : Uses BCrypt for hashing              │  │
│  │  + LoginAsync() : Verifies with BCrypt                    │  │
│  │  + GenerateJwtToken() : Creates JWT with claims           │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                      Domain Models                               │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  User                                                     │  │
│  │  ────                                                     │  │
│  │  + Id : Guid                                              │  │
│  │  + Username : string                                      │  │
│  │  + Email : string                                         │  │
│  │  + PasswordHash : string                                  │  │
│  │  + CreatedAt : DateTime                                   │  │
│  │  + LastLoginAt : DateTime?                                │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘
```

### Middleware Pipeline

```
HTTP Request
     │
     ▼
┌─────────────────────┐
│  UseHttpsRedirection │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│ UseAuthentication   │ ◄── Validates JWT Token
│  - Reads Bearer     │     Extracts User Claims
│  - Validates Token  │     Sets User Principal
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│  UseAuthorization   │ ◄── Checks [Authorize] Attribute
│  - Checks Claims    │     Allows [AllowAnonymous]
│  - Enforces Policies│     Returns 401/403 if needed
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│   MapControllers    │
│  Route to Controller│
└─────────────────────┘
```

---

## 💾 Database Schema

### Users Table Schema

```sql
CREATE TABLE Users (
    Id              UNIQUEIDENTIFIER    PRIMARY KEY DEFAULT NEWID(),
    Username        NVARCHAR(100)       NOT NULL,
    Email           NVARCHAR(100)       NOT NULL UNIQUE,
    PasswordHash    NVARCHAR(MAX)       NOT NULL,
    CreatedAt       DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    LastLoginAt     DATETIME2           NULL,
    
    CONSTRAINT UK_Users_Email UNIQUE (Email),
    INDEX IX_Users_Email (Email)
);
```

### Entity Relationship Diagram

```
┌─────────────────────┐
│       Users         │
│─────────────────────│
│ Id (PK)             │
│ Username            │
│ Email (UK)          │
│ PasswordHash        │
│ CreatedAt           │
│ LastLoginAt         │
└─────────────────────┘
         │
         │ (Future: Can be linked to)
         │
         ▼
┌─────────────────────┐         ┌─────────────────────┐
│      Regions        │         │       Walks         │
│─────────────────────│         │─────────────────────│
│ Id (PK)             │         │ Id (PK)             │
│ Name                │         │ Name                │
│ Code                │         │ Description         │
│ RegionImageUrl      │◄────────│ RegionId (FK)       │
└─────────────────────┘         │ DifficultyId (FK)   │
                                │ LengthInKm          │
                                │ WalkImageUrl        │
                                └─────────────────────┘
                                         │
                                         │
                                         ▼
                                ┌─────────────────────┐
                                │   Difficulties      │
                                │─────────────────────│
                                │ Id (PK)             │
                                │ Name                │
                                └─────────────────────┘
```

### Password Storage Strategy

```
┌──────────────────────────────────────────────────────────────┐
│               Password Hashing with BCrypt                    │
├──────────────────────────────────────────────────────────────┤
│                                                               │
│  Plain Text Password: "password123"                          │
│           ↓                                                   │
│  BCrypt.HashPassword("password123")                          │
│           ↓                                                   │
│  Hashed: "$2a$11$Xo5JQq..." (60 characters)                  │
│           ↓                                                   │
│  Stored in Database (PasswordHash column)                    │
│                                                               │
│  ─────────────────────────────────────────────                │
│                                                               │
│  Login Attempt: "password123"                                │
│           ↓                                                   │
│  BCrypt.Verify("password123", "$2a$11$Xo5JQq...")           │
│           ↓                                                   │
│  Result: true/false                                          │
│                                                               │
│  ✅ Benefits:                                                 │
│    • Salt automatically generated                            │
│    • Computationally expensive (protects from brute force)   │
│    • Industry standard                                       │
│    • One-way hashing (irreversible)                          │
│                                                               │
└──────────────────────────────────────────────────────────────┘
```

---

## 🌐 API Endpoints

### Authentication Endpoints

#### 1. Register New User
```http
POST /api/auth/register
Content-Type: application/json

Request:
{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecurePass123"
}

Response (200 OK):
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "john_doe",
  "email": "john@example.com"
}

Error (400 Bad Request):
{
  "message": "User with this email already exists"
}
```

#### 2. Login
```http
POST /api/auth/login
Content-Type: application/json

Request:
{
  "email": "john@example.com",
  "password": "SecurePass123"
}

Response (200 OK):
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "john_doe",
  "email": "john@example.com"
}

Error (401 Unauthorized):
{
  "message": "Invalid email or password"
}
```

### Protected Endpoints Example

#### Create Region (Protected)
```http
POST /api/region
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

Request:
{
  "name": "Rajasthan",
  "code": "RJ",
  "regionImageUrl": "https://example.com/rajasthan.jpg"
}

Response (201 Created):
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Rajasthan",
  "code": "RJ",
  "regionImageUrl": "https://example.com/rajasthan.jpg"
}

Error (401 Unauthorized) - If no token or invalid token:
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

### API Endpoint Matrix

| Endpoint | Method | Auth Required | Description |
|----------|--------|---------------|-------------|
| `/api/auth/register` | POST | ❌ No | Register new user |
| `/api/auth/login` | POST | ❌ No | Login user |
| `/api/region` | GET | ❌ No | Get all regions |
| `/api/region/{id}` | GET | ❌ No | Get single region |
| `/api/region` | POST | ✅ Yes | Create region |
| `/api/region/{id}` | PUT | ✅ Yes | Update region |
| `/api/region/{id}` | DELETE | ✅ Yes | Delete region |
| `/api/walks` | GET | ❌ No | Get all walks |
| `/api/walks/{id}` | GET | ❌ No | Get single walk |
| `/api/walks` | POST | ✅ Yes | Create walk |
| `/api/walks/{id}` | PUT | ✅ Yes | Update walk |
| `/api/walks/{id}` | DELETE | ✅ Yes | Delete walk |

---

## 🔒 Security Implementation

### JWT Token Structure

```
┌─────────────────────────────────────────────────────────────┐
│                     JWT Token Anatomy                        │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  eyJhbGc...    .    eyJzdWI...    .    SflKxwRJ...          │
│     │                   │                   │                │
│     │                   │                   │                │
│  HEADER            PAYLOAD            SIGNATURE             │
│                                                              │
└─────────────────────────────────────────────────────────────┘

┌──────────────────┐
│     HEADER       │
├──────────────────┤
│ {                │
│   "alg": "HS256",│ ◄── HMAC SHA-256 Algorithm
│   "typ": "JWT"   │ ◄── Token Type
│ }                │
└──────────────────┘

┌──────────────────────────────┐
│          PAYLOAD             │
├──────────────────────────────┤
│ {                            │
│   "nameid": "guid...",       │ ◄── User ID
│   "unique_name": "john_doe", │ ◄── Username
│   "email": "john@ex.com",    │ ◄── Email
│   "sub": "john@ex.com",      │ ◄── Subject
│   "jti": "token-id...",      │ ◄── Token ID
│   "exp": 1735689600,         │ ◄── Expiration (Unix timestamp)
│   "iss": "https://...",      │ ◄── Issuer
│   "aud": "https://..."       │ ◄── Audience
│ }                            │
└──────────────────────────────┘

┌──────────────────────────────────────┐
│           SIGNATURE                  │
├──────────────────────────────────────┤
│  HMACSHA256(                         │
│    base64UrlEncode(header) + "." +   │
│    base64UrlEncode(payload),         │
│    secret_key                        │
│  )                                   │
│                                      │
│  ✅ Ensures token integrity          │
│  ✅ Prevents tampering                │
└──────────────────────────────────────┘
```

### Security Features

```
┌─────────────────────────────────────────────────────────────┐
│                  Security Layer Matrix                       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  Layer 1: HTTPS/TLS                                         │
│  ─────────────────────                                      │
│  ✅ All traffic encrypted                                    │
│  ✅ Tokens never sent in plain text                          │
│  ✅ Man-in-the-middle protection                             │
│                                                              │
│  Layer 2: Password Hashing (BCrypt)                         │
│  ────────────────────────────────────                       │
│  ✅ One-way hashing (cannot be reversed)                     │
│  ✅ Automatic salt generation                                │
│  ✅ Configurable work factor                                 │
│  ✅ Protection against rainbow tables                        │
│                                                              │
│  Layer 3: JWT Validation                                    │
│  ───────────────────────                                    │
│  ✅ Signature verification                                   │
│  ✅ Expiration check (7 days)                                │
│  ✅ Issuer validation                                        │
│  ✅ Audience validation                                      │
│                                                              │
│  Layer 4: Authorization                                     │
│  ─────────────────────                                      │
│  ✅ [Authorize] attribute enforcement                        │
│  ✅ Claims-based access control                              │
│  ✅ Role-based access (future enhancement)                   │
│                                                              │
│  Layer 5: Input Validation                                  │
│  ────────────────────────                                   │
│  ✅ FluentValidation rules                                   │
│  ✅ Email format validation                                  │
│  ✅ Password minimum length (6 chars)                        │
│  ✅ SQL injection prevention (EF Core)                       │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### Token Validation Process

```mermaid
flowchart TD
    A[Client sends request with JWT] --> B{Token Present?}
    B -->|No| C[401 Unauthorized]
    B -->|Yes| D{Token Format Valid?}
    D -->|No| C
    D -->|Yes| E{Signature Valid?}
    E -->|No| C
    E -->|Yes| F{Token Expired?}
    F -->|Yes| C
    F -->|No| G{Issuer Valid?}
    G -->|No| C
    G -->|Yes| H{Audience Valid?}
    H -->|No| C
    H -->|Yes| I[Extract Claims]
    I --> J[Set User Principal]
    J --> K{Controller has [Authorize]?}
    K -->|No| L[Allow Request]
    K -->|Yes| M{User Authenticated?}
    M -->|No| C
    M -->|Yes| L
    L --> N[Execute Controller Action]
```

---

## 🔄 Request/Response Flow

### Successful Authentication Flow

```
┌──────────────────────────────────────────────────────────────────┐
│                  Step-by-Step Execution Flow                      │
├──────────────────────────────────────────────────────────────────┤
│                                                                   │
│  STEP 1: Client Registration                                     │
│  ───────────────────────────────────────────────────────────     │
│  Flutter App → POST /api/auth/register                           │
│  {                                                                │
│    "username": "john_doe",                                        │
│    "email": "john@example.com",                                   │
│    "password": "SecurePass123"                                    │
│  }                                                                │
│                                                                   │
│  ▼                                                                │
│                                                                   │
│  STEP 2: Server Validation                                       │
│  ────────────────────────                                        │
│  • FluentValidation checks input                                 │
│  • Email format verified                                         │
│  • Password length checked (min 6 chars)                         │
│  • Check if user already exists                                  │
│                                                                   │
│  ▼                                                                │
│                                                                   │
│  STEP 3: Password Hashing                                        │
│  ───────────────────────                                         │
│  BCrypt.HashPassword("SecurePass123")                            │
│  Result: "$2a$11$Xo5JQq..." (60 chars)                           │
│                                                                   │
│  ▼                                                                │
│                                                                   │
│  STEP 4: Database Insert                                         │
│  ──────────────────────                                          │
│  INSERT INTO Users (Id, Username, Email, PasswordHash, ...)      │
│  VALUES (NEWID(), 'john_doe', 'john@example.com', ...)           │
│                                                                   │
│  ▼                                                                │
│                                                                   │
│  STEP 5: JWT Token Generation                                    │
│  ────────────────────────────                                    │
│  Claims:                                                          │
│    • NameIdentifier: user.Id                                     │
│    • Name: user.Username                                         │
│    • Email: user.Email                                           │
│    • Sub: user.Email                                             │
│    • Jti: new GUID                                               │
│  Expiration: 7 days                                              │
│  Signing: HMAC-SHA256 with secret key                            │
│                                                                   │
│  ▼                                                                │
│                                                                   │
│  STEP 6: Response to Client                                      │
│  ─────────────────────────                                       │
│  200 OK                                                           │
│  {                                                                │
│    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",           │
│    "username": "john_doe",                                        │
│    "email": "john@example.com"                                    │
│  }                                                                │
│                                                                   │
│  ▼                                                                │
│                                                                   │
│  STEP 7: Client Stores Token                                     │
│  ───────────────────────────                                     │
│  Flutter App → Secure Storage                                    │
│  • flutter_secure_storage package                                │
│  • Encrypted on device                                           │
│                                                                   │
│  ▼                                                                │
│                                                                   │
│  STEP 8: Subsequent Requests                                     │
│  ─────────────────────────                                       │
│  POST /api/region                                                │
│  Headers:                                                         │
│    Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9... │
│                                                                   │
│  ▼                                                                │
│                                                                   │
│  STEP 9: Token Validation                                        │
│  ───────────────────────                                         │
│  • JWT Middleware validates token                                │
│  • Extracts claims                                               │
│  • Sets HttpContext.User                                         │
│                                                                   │
│  ▼                                                                │
│                                                                   │
│  STEP 10: Authorization Check                                    │
│  ────────────────────────────                                    │
│  • Controller has [Authorize] attribute                          │
│  • User is authenticated ✅                                       │
│  • Request proceeds                                              │
│                                                                   │
└──────────────────────────────────────────────────────────────────┘
```

### Error Handling Flow

```
┌────────────────────────────────────────────────────────────┐
│                     Error Scenarios                         │
├────────────────────────────────────────────────────────────┤
│                                                             │
│  Scenario 1: Invalid Credentials                           │
│  ────────────────────────────                              │
│  POST /api/auth/login                                      │
│  { "email": "john@ex.com", "password": "wrong" }           │
│         ↓                                                   │
│  BCrypt.Verify fails                                       │
│         ↓                                                   │
│  401 Unauthorized                                          │
│  { "message": "Invalid email or password" }                │
│                                                             │
│  ────────────────────────────────────────────               │
│                                                             │
│  Scenario 2: Missing Token                                 │
│  ───────────────────────                                   │
│  POST /api/region (no Authorization header)                │
│         ↓                                                   │
│  JWT Middleware: No token found                            │
│         ↓                                                   │
│  401 Unauthorized                                          │
│                                                             │
│  ────────────────────────────────────────────               │
│                                                             │
│  Scenario 3: Expired Token                                 │
│  ────────────────────────                                  │
│  POST /api/region (token issued 8 days ago)                │
│         ↓                                                   │
│  JWT Middleware: Token expired                             │
│         ↓                                                   │
│  401 Unauthorized                                          │
│                                                             │
│  ────────────────────────────────────────────               │
│                                                             │
│  Scenario 4: Tampered Token                                │
│  ─────────────────────────                                 │
│  POST /api/region (modified token payload)                 │
│         ↓                                                   │
│  JWT Middleware: Signature verification fails              │
│         ↓                                                   │
│  401 Unauthorized                                          │
│                                                             │
│  ────────────────────────────────────────────               │
│                                                             │
│  Scenario 5: User Already Exists                           │
│  ──────────────────────────────                            │
│  POST /api/auth/register (duplicate email)                 │
│         ↓                                                   │
│  Database check: User exists                               │
│         ↓                                                   │
│  400 Bad Request                                           │
│  { "message": "User with this email already exists" }      │
│                                                             │
└────────────────────────────────────────────────────────────┘
```

---

## 🛠️ Technology Stack

### Backend Technologies

```
┌─────────────────────────────────────────────────────────────┐
│                    Technology Stack                          │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  Framework & Runtime                                        │
│  ───────────────────                                        │
│  • .NET 8.0                                                 │
│  • ASP.NET Core Web API                                     │
│  • C# 12                                                    │
│                                                              │
│  Authentication & Security                                  │
│  ─────────────────────────                                  │
│  • Microsoft.AspNetCore.Authentication.JwtBearer (8.0.11)   │
│  • System.IdentityModel.Tokens.Jwt (8.14.0)                 │
│  • Microsoft.IdentityModel.Tokens (8.14.0)                  │
│  • BCrypt.Net-Next (4.0.3)                                  │
│                                                              │
│  Data Access                                                │
│  ────────────                                               │
│  • Entity Framework Core (9.0.9)                            │
│  • Microsoft.EntityFrameworkCore.SqlServer (9.0.9)          │
│  • Microsoft.EntityFrameworkCore.Tools (9.0.9)              │
│                                                              │
│  Validation & Mapping                                       │
│  ──────────────────────                                     │
│  • FluentValidation.AspNetCore (11.3.1)                     │
│  • AutoMapper (13.0.1)                                      │
│                                                              │
│  API Documentation                                          │
│  ──────────────────                                         │
│  • Swashbuckle.AspNetCore (6.6.2)                           │
│  • Swagger/OpenAPI                                          │
│                                                              │
│  Database                                                   │
│  ────────                                                   │
│  • Microsoft SQL Server (LocalDB or Full)                   │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### Client Technologies (Flutter Integration)

```
┌─────────────────────────────────────────────────────────────┐
│              Flutter Mobile App Integration                  │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  Required Packages                                          │
│  ─────────────────                                          │
│  • http or dio (for API calls)                              │
│  • flutter_secure_storage (for token storage)               │
│  • provider or bloc (for state management)                  │
│                                                              │
│  Implementation Example:                                    │
│  ──────────────────────                                     │
│                                                              │
│  // 1. Login and store token                                │
│  final response = await http.post(                          │
│    Uri.parse('https://api.indiatrails.com/api/auth/login'), │
│    body: json.encode({                                      │
│      'email': email,                                        │
│      'password': password                                   │
│    }),                                                       │
│  );                                                          │
│                                                              │
│  final token = json.decode(response.body)['token'];         │
│  await storage.write(key: 'jwt_token', value: token);       │
│                                                              │
│  // 2. Use token in subsequent requests                     │
│  final token = await storage.read(key: 'jwt_token');        │
│  final response = await http.post(                          │
│    Uri.parse('https://api.indiatrails.com/api/region'),     │
│    headers: {                                               │
│      'Authorization': 'Bearer $token',                      │
│      'Content-Type': 'application/json'                     │
│    },                                                        │
│    body: json.encode(regionData),                           │
│  );                                                          │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## 🚀 Deployment Considerations

### Development Environment

```
┌─────────────────────────────────────────────────────────────┐
│              Development Configuration                       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  appsettings.Development.json                               │
│  ────────────────────────────                               │
│  {                                                           │
│    "Jwt": {                                                  │
│      "Key": "Dev-Secret-Key-32-Chars-Long-Min",             │
│      "Issuer": "https://localhost:7000",                    │
│      "Audience": "https://localhost:7000"                   │
│    },                                                        │
│    "ConnectionStrings": {                                   │
│      "IndiaTrailsDBConnectionString": "Server=localhost..." │
│    }                                                         │
│  }                                                           │
│                                                              │
│  ✅ Use User Secrets for sensitive data                      │
│  ✅ Shorter token expiration for testing                     │
│  ✅ Detailed logging enabled                                 │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### Production Environment

```
┌─────────────────────────────────────────────────────────────┐
│             Production Configuration                         │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  Environment Variables (Azure, AWS, etc.)                   │
│  ────────────────────────────────────────                   │
│  JWT__KEY=<strong-random-key-from-key-vault>                │
│  JWT__ISSUER=https://api.indiatrails.com                    │
│  JWT__AUDIENCE=https://api.indiatrails.com                  │
│  ConnectionStrings__IndiaTrailsDBConnectionString=...       │
│                                                              │
│  ✅ Store secrets in Azure Key Vault                         │
│  ✅ Use managed identities                                   │
│  ✅ Enable HTTPS only                                        │
│  ✅ Configure CORS properly                                  │
│  ✅ Enable rate limiting                                     │
│  ✅ Set up monitoring and alerts                             │
│                                                              │
│  Security Enhancements:                                     │
│  ──────────────────────                                     │
│  • Shorter token expiration (1 hour)                        │
│  • Implement refresh tokens                                 │
│  • Add IP whitelisting if needed                            │
│  • Enable request throttling                                │
│  • Set up WAF (Web Application Firewall)                    │
│  • Regular security audits                                  │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### Scalability Architecture

```
                        ┌────────────────────┐
                        │   Load Balancer    │
                        │   (Azure LB/AWS)   │
                        └─────────┬──────────┘
                                  │
                    ┌─────────────┴─────────────┐
                    │                           │
          ┌─────────▼─────────┐       ┌────────▼────────┐
          │   API Instance 1   │       │  API Instance 2  │
          │  (Stateless JWT)   │       │ (Stateless JWT)  │
          └─────────┬──────────┘       └────────┬─────────┘
                    │                           │
                    └─────────────┬─────────────┘
                                  │
                        ┌─────────▼──────────┐
                        │   SQL Server       │
                        │   (Managed)        │
                        └────────────────────┘
                        
Benefits:
✅ Stateless authentication (no session storage)
✅ Horizontal scaling without session affinity
✅ No shared session storage needed
✅ Easy to add/remove instances
```

### Monitoring & Logging

```
┌─────────────────────────────────────────────────────────────┐
│                  Recommended Monitoring                      │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  Authentication Events to Log:                              │
│  ─────────────────────────────                              │
│  • Successful registrations                                 │
│  • Failed registration attempts                             │
│  • Successful logins (with timestamp)                       │
│  • Failed login attempts (track brute force)                │
│  • Token expiration events                                  │
│  • Invalid token attempts                                   │
│                                                              │
│  Metrics to Monitor:                                        │
│  ───────────────────                                        │
│  • Login success/failure rate                               │
│  • Average response time                                    │
│  • Token validation failures                                │
│  • Concurrent authenticated users                           │
│  • API endpoint usage by auth status                        │
│                                                              │
│  Tools:                                                     │
│  ──────                                                     │
│  • Application Insights (Azure)                             │
│  • CloudWatch (AWS)                                         │
│  • Serilog for structured logging                           │
│  • ELK Stack (Elasticsearch, Logstash, Kibana)              │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## 📋 Implementation Checklist

### Phase 1: Development ✅

- [x] Install required NuGet packages
- [x] Create User domain model
- [x] Create authentication DTOs
- [x] Implement AuthRepository
- [x] Create AuthController
- [x] Configure JWT in Program.cs
- [x] Update DbContext
- [x] Add database migration
- [x] Apply [Authorize] to controllers
- [x] Test registration endpoint
- [x] Test login endpoint
- [x] Test protected endpoints

### Phase 2: Testing ⏳

- [ ] Write unit tests for AuthRepository
- [ ] Write integration tests for AuthController
- [ ] Test token expiration scenarios
- [ ] Test invalid token scenarios
- [ ] Test concurrent user sessions
- [ ] Load testing with multiple users
- [ ] Security penetration testing

### Phase 3: Production Readiness 📋

- [ ] Move JWT key to Azure Key Vault
- [ ] Configure production connection string
- [ ] Set up HTTPS/SSL certificates
- [ ] Configure CORS for production domains
- [ ] Implement rate limiting
- [ ] Set up logging and monitoring
- [ ] Create API documentation
- [ ] Security audit
- [ ] Performance optimization
- [ ] Backup and disaster recovery plan

### Phase 4: Future Enhancements 🚀

- [ ] Implement refresh tokens
- [ ] Add email verification
- [ ] Implement password reset
- [ ] Add two-factor authentication
- [ ] Role-based access control (Admin, User)
- [ ] OAuth integration (Google, Facebook)
- [ ] Token revocation mechanism
- [ ] User profile management
- [ ] Activity logging per user
- [ ] API versioning

---

## 📞 Support & Maintenance

### Common Issues

1. **401 Unauthorized despite valid token**
   - Verify `UseAuthentication()` is before `UseAuthorization()`
   - Check token format: `Bearer {token}`
   - Validate JWT settings match

2. **Token validation fails**
   - Ensure Issuer/Audience match
   - Check JWT key consistency
   - Verify token hasn't expired

3. **Performance concerns**
   - JWT validation is fast (no DB lookup)
   - Consider shorter expiration + refresh tokens
   - Implement caching where appropriate

### Maintenance Tasks

- Regular security updates for packages
- Monitor failed authentication attempts
- Review and rotate JWT secret keys
- Audit user accounts periodically
- Update token expiration policies
- Performance monitoring and optimization

---

## 📚 References & Documentation

- **JWT.io** - https://jwt.io - Decode and verify JWTs
- **BCrypt** - https://github.com/BcryptNet/bcrypt.net - Password hashing
- **Microsoft Docs** - https://docs.microsoft.com/aspnet/core/security/
- **OWASP** - https://owasp.org - Security best practices

---

**Document Version:** 1.0  
**Last Updated:** October 2025  
**Prepared By:** Development Team  
**Project:** IndiaTrails.API  
**Technology:** .NET 8.0, JWT Authentication, SQL Server
