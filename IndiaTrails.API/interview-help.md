# .NET Core Web API - Complete Interview Guide

## 📚 Table of Contents
1. [.NET Core Fundamentals](#1-net-core-fundamentals)
2. [Web API Basics](#2-web-api-basics)
3. [Dependency Injection (DI)](#3-dependency-injection)
4. [Middleware Pipeline](#4-middleware-pipeline)
5. [Routing & Model Binding](#5-routing--model-binding)
6. [Authentication & Authorization](#6-authentication--authorization)
7. [Entity Framework Core](#7-entity-framework-core)
8. [API Versioning](#8-api-versioning)
9. [Error Handling & Logging](#9-error-handling--logging)
10. [Performance & Optimization](#10-performance--optimization)
11. [Design Patterns & Architecture](#11-design-patterns--architecture)
12. [Testing](#12-testing)
13. [Advanced Topics](#13-advanced-topics)

---

## 1. .NET Core Fundamentals

### Q: What is .NET Core? How is it different from .NET Framework?

**Answer:**
.NET Core is a **cross-platform, open-source framework** for building modern applications.

| Feature | .NET Core | .NET Framework |
|---------|-----------|----------------|
| Platform | Cross-platform (Windows, Linux, macOS) | Windows only |
| Open Source | ✅ Yes | ❌ No |
| Performance | ⚡ High performance | Moderate |
| Deployment | Side-by-side installation | System-wide |
| CLI | ✅ dotnet CLI | ❌ Limited |
| Microservices | ✅ Optimized | ⚠️ Possible |

**Key Points:**
- **.NET 5+** unified .NET Core and .NET Framework
- **ASP.NET Core** is the web framework
- **Cross-platform** development and deployment

---

### Q: What is Kestrel?

**Answer:**
**Kestrel** is a cross-platform web server for ASP.NET Core.

**Key Features:**
- Built-in web server
- High performance (async I/O)
- Cross-platform
- Can be used standalone or with reverse proxy (IIS, Nginx, Apache)

**Architecture:**
```
Internet → Reverse Proxy (Nginx/IIS) → Kestrel → ASP.NET Core App
```

**Configuration:**
```csharp
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 52428800; // 50 MB
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
});
```

---

### Q: Explain the Program.cs file in .NET 6+

**Answer:**
**.NET 6+** uses **minimal hosting model** with top-level statements.

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Configure Services (DI Container)
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<IProductService, ProductService>();

// 2. Build the app
var app = builder.Build();

// 3. Configure Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// 4. Run the application
app.Run();
```

**Key Sections:**
1. **WebApplicationBuilder** - Configure services
2. **WebApplication** - Configure middleware
3. **app.Run()** - Start the application

---

## 2. Web API Basics

### Q: What is REST? Explain RESTful principles.

**Answer:**
**REST** (Representational State Transfer) is an architectural style for web services.

**6 Principles:**

1. **Client-Server** - Separation of concerns
2. **Stateless** - No session stored on server
3. **Cacheable** - Responses can be cached
4. **Uniform Interface** - Standard HTTP methods
5. **Layered System** - Multiple architectural layers
6. **Code on Demand** (Optional) - Executable code transfer

**HTTP Methods:**
```csharp
[HttpGet]    // Read - GET /api/products
[HttpPost]   // Create - POST /api/products
[HttpPut]    // Update (full) - PUT /api/products/1
[HttpPatch]  // Update (partial) - PATCH /api/products/1
[HttpDelete] // Delete - DELETE /api/products/1
```

---

### Q: What is the difference between [ApiController] and regular Controller?

**Answer:**

| Feature | [ApiController] | Regular Controller |
|---------|----------------|-------------------|
| **Attribute Routing** | Required | Optional |
| **Model Validation** | Automatic (400 response) | Manual checking |
| **Binding Source** | Inferred automatically | Must specify |
| **Problem Details** | Returns RFC 7807 format | Basic error response |
| **Multipart/form-data** | Inferred | Must specify |

**Example:**
```csharp
[ApiController]  // Enables API-specific behaviors
[Route("api/[controller]")]
public class ProductsController : ControllerBase  // Use ControllerBase for APIs
{
    [HttpPost]
    public IActionResult Create(ProductDto product)  // [FromBody] inferred
    {
        // ModelState validation automatic
        if (!ModelState.IsValid)  // This is handled automatically!
            return BadRequest(ModelState);
            
        return Ok(product);
    }
}
```

**Without [ApiController]:**
```csharp
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] ProductDto product)  // Must specify
    {
        if (!ModelState.IsValid)  // Must check manually
            return BadRequest(ModelState);
            
        return Ok(product);
    }
}
```

---

### Q: What are Action Results in Web API?

**Answer:**
**Action Results** represent the response from controller actions.

**Common Types:**

```csharp
public class ProductsController : ControllerBase
{
    // 1. OkResult (200)
    [HttpGet("ok")]
    public IActionResult GetOk()
    {
        return Ok(new { message = "Success" });
    }
    
    // 2. CreatedAtAction (201)
    [HttpPost]
    public IActionResult Create(Product product)
    {
        var created = _service.Create(product);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
    
    // 3. NoContent (204)
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.Delete(id);
        return NoContent();
    }
    
    // 4. BadRequest (400)
    [HttpGet("bad")]
    public IActionResult GetBad()
    {
        return BadRequest("Invalid request");
    }
    
    // 5. NotFound (404)
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var product = _service.GetById(id);
        if (product == null)
            return NotFound();
        return Ok(product);
    }
    
    // 6. Unauthorized (401)
    [HttpGet("secure")]
    public IActionResult GetSecure()
    {
        return Unauthorized();
    }
    
    // 7. Forbid (403)
    [HttpGet("admin")]
    public IActionResult GetAdmin()
    {
        return Forbid();
    }
}
```

**Status Codes Quick Reference:**
- **2xx Success**: 200 OK, 201 Created, 204 No Content
- **4xx Client Error**: 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found
- **5xx Server Error**: 500 Internal Server Error

---

### Q: What is Content Negotiation?

**Answer:**
**Content Negotiation** = Server returns data in the format requested by client.

**How it works:**
```
Client Request:
GET /api/products
Accept: application/json  ← Client specifies format

Server Response:
Content-Type: application/json  ← Server confirms format
{ "id": 1, "name": "Product" }
```

**Configuration:**
```csharp
builder.Services.AddControllers()
    .AddXmlSerializerFormatters()  // Add XML support
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;  // PascalCase
    });
```

**Controller Example:**
```csharp
[HttpGet]
[Produces("application/json", "application/xml")]  // Supports both
public IActionResult GetProducts()
{
    return Ok(products);
}
```

---

## 3. Dependency Injection

### Q: What is Dependency Injection? Explain service lifetimes.

**Answer:**
**Dependency Injection (DI)** = Design pattern where objects receive dependencies from external source rather than creating them.

**Benefits:**
- ✅ Loose coupling
- ✅ Testability
- ✅ Maintainability
- ✅ Reusability

**3 Service Lifetimes:**

```csharp
// 1. Transient - New instance every time
builder.Services.AddTransient<IEmailService, EmailService>();
// Use: Lightweight, stateless services

// 2. Scoped - One instance per HTTP request
builder.Services.AddScoped<IOrderService, OrderService>();
// Use: Database contexts, repository per request

// 3. Singleton - One instance for application lifetime
builder.Services.AddSingleton<ICacheService, CacheService>();
// Use: Logging, configuration, caching
```

**Comparison:**

| Lifetime | Created | Disposed | Use Case |
|----------|---------|----------|----------|
| **Transient** | Every time | Immediately after use | Lightweight services |
| **Scoped** | Per request | End of request | DbContext, Repositories |
| **Singleton** | First use | App shutdown | Logging, Config, Cache |

**Example:**
```csharp
// Service Interface
public interface IProductService
{
    Task<List<Product>> GetAllAsync();
}

// Service Implementation
public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    
    public ProductService(AppDbContext context)  // DI via constructor
    {
        _context = context;
    }
    
    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }
}

// Register in Program.cs
builder.Services.AddScoped<IProductService, ProductService>();

// Use in Controller
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    
    public ProductsController(IProductService productService)  // Injected
    {
        _productService = productService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }
}
```

---

### Q: What happens if you inject Scoped service into Singleton?

**Answer:**
⚠️ **Captive Dependency Problem** - This is a **mistake** that causes issues!

**Problem:**
```csharp
// ❌ BAD: Singleton capturing Scoped service
public class MySingleton
{
    private readonly IOrderService _orderService;  // Scoped service
    
    public MySingleton(IOrderService orderService)
    {
        _orderService = orderService;  // Lives forever in Singleton!
    }
}

builder.Services.AddSingleton<MySingleton>();
builder.Services.AddScoped<IOrderService, OrderService>();
```

**Issues:**
- Scoped service lives as long as Singleton
- DbContext stays open (memory leak)
- Stale data across requests
- Concurrency issues

**Solution 1: Use IServiceProvider**
```csharp
public class MySingleton
{
    private readonly IServiceProvider _serviceProvider;
    
    public MySingleton(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public void DoWork()
    {
        using var scope = _serviceProvider.CreateScope();
        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
        // Use orderService
    }
}
```

**Solution 2: Change Singleton to Scoped**
```csharp
builder.Services.AddScoped<MySingleton>();  // ✅ Match lifetimes
builder.Services.AddScoped<IOrderService, OrderService>();
```

---

## 4. Middleware Pipeline

### Q: What is Middleware? Explain the pipeline.

**Answer:**
**Middleware** = Components that handle HTTP requests/responses in a pipeline.

**Pipeline Flow:**
```
Request  →  Middleware 1  →  Middleware 2  →  Endpoint  →  Middleware 2  →  Middleware 1  →  Response
           ↓                ↓                   ↓             ↑                ↑
           Execute          Execute             Execute      Execute          Execute
```

**Common Middleware (Order Matters!):**
```csharp
var app = builder.Build();

// 1. Exception Handling (First)
app.UseExceptionHandler("/error");

// 2. HTTPS Redirection
app.UseHttpsRedirection();

// 3. Static Files
app.UseStaticFiles();

// 4. Routing
app.UseRouting();

// 5. CORS (Before Authentication)
app.UseCors();

// 6. Authentication (Before Authorization)
app.UseAuthentication();

// 7. Authorization
app.UseAuthorization();

// 8. Endpoints (Last)
app.MapControllers();

app.Run();
```

**⚠️ Order is Critical!**
```csharp
// ❌ WRONG - Authorization before Authentication
app.UseAuthorization();   // Won't work!
app.UseAuthentication();

// ✅ CORRECT
app.UseAuthentication();
app.UseAuthorization();
```

---

### Q: How to create custom middleware?

**Answer:**

**Method 1: Inline Middleware**
```csharp
app.Use(async (context, next) =>
{
    // Before request
    Console.WriteLine($"Request: {context.Request.Path}");
    
    await next.Invoke();  // Call next middleware
    
    // After request
    Console.WriteLine($"Response: {context.Response.StatusCode}");
});
```

**Method 2: Custom Middleware Class**
```csharp
// Middleware Class
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // Before request
        _logger.LogInformation($"Handling request: {context.Request.Path}");
        
        await _next(context);  // Next middleware
        
        // After request
        _logger.LogInformation($"Finished handling request: {context.Response.StatusCode}");
    }
}

// Extension Method
public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}

// Register in Program.cs
app.UseRequestLogging();
```

**Real-World Example: Request Timing**
```csharp
public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    
    public RequestTimingMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        await _next(context);
        
        stopwatch.Stop();
        context.Response.Headers.Add("X-Response-Time", $"{stopwatch.ElapsedMilliseconds}ms");
    }
}
```

---

## 5. Routing & Model Binding

### Q: Explain Attribute Routing vs Convention Routing

**Answer:**

**Attribute Routing** (Recommended for APIs):
```csharp
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]  // GET /api/products
    public IActionResult GetAll() { }
    
    [HttpGet("{id}")]  // GET /api/products/5
    public IActionResult GetById(int id) { }
    
    [HttpGet("featured")]  // GET /api/products/featured
    public IActionResult GetFeatured() { }
    
    [HttpGet("category/{category}")]  // GET /api/products/category/electronics
    public IActionResult GetByCategory(string category) { }
}
```

**Convention Routing** (Traditional MVC):
```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

**Best Practice:** Use **Attribute Routing** for Web APIs

---

### Q: What is Model Binding? Explain binding sources.

**Answer:**
**Model Binding** = Automatic mapping of HTTP request data to action parameters.

**Binding Sources:**

```csharp
public class ProductsController : ControllerBase
{
    // 1. [FromRoute] - URL path
    [HttpGet("{id}")]
    public IActionResult Get([FromRoute] int id) { }
    
    // 2. [FromQuery] - Query string
    [HttpGet]
    public IActionResult Search([FromQuery] string name, [FromQuery] int page = 1) { }
    // GET /api/products?name=laptop&page=2
    
    // 3. [FromBody] - Request body (JSON)
    [HttpPost]
    public IActionResult Create([FromBody] ProductDto product) { }
    
    // 4. [FromHeader] - HTTP headers
    [HttpGet]
    public IActionResult GetWithHeader([FromHeader(Name = "X-User-Id")] string userId) { }
    
    // 5. [FromForm] - Form data
    [HttpPost("upload")]
    public IActionResult Upload([FromForm] IFormFile file, [FromForm] string description) { }
    
    // 6. [FromServices] - Dependency Injection
    [HttpGet]
    public IActionResult GetAll([FromServices] IProductService service)
    {
        return Ok(service.GetAll());
    }
}
```

**Complex Model Binding:**
```csharp
// Query Object
public class ProductQuery
{
    public string Name { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

[HttpGet]
public IActionResult Search([FromQuery] ProductQuery query)
{
    // GET /api/products?name=laptop&minPrice=100&maxPrice=1000&page=1&pageSize=20
    return Ok(query);
}
```

---

### Q: What is Model Validation? How to validate models?

**Answer:**
**Model Validation** = Ensuring incoming data meets requirements.

**Data Annotations:**
```csharp
public class ProductDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }
    
    [Required]
    [Range(0.01, 10000)]
    public decimal Price { get; set; }
    
    [Required]
    [EmailAddress]
    public string ContactEmail { get; set; }
    
    [Url]
    public string Website { get; set; }
    
    [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Invalid phone format")]
    public string Phone { get; set; }
    
    [Compare("Email")]
    public string ConfirmEmail { get; set; }
}
```

**Controller Validation:**
```csharp
[ApiController]  // Automatic validation
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpPost]
    public IActionResult Create(ProductDto product)
    {
        // With [ApiController], automatic 400 response if invalid
        // Without [ApiController], need manual check:
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        return Ok(product);
    }
}
```

**FluentValidation (Advanced):**
```csharp
// Install: FluentValidation.AspNetCore
public class ProductValidator : AbstractValidator<ProductDto>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(3, 100);
            
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .LessThan(10000);
            
        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email));
    }
}

// Register
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<ProductDto>, ProductValidator>();
```

---

## 6. Authentication & Authorization

### Q: Difference between Authentication and Authorization?

**Answer:**

| Concept | Definition | Question | Example |
|---------|------------|----------|---------|
| **Authentication** | Verifying identity | "Who are you?" | Login with username/password |
| **Authorization** | Verifying permissions | "What can you do?" | Admin can delete, User can view |

**Flow:**
```
1. Authentication → Login → Token/Cookie
2. Authorization → Check permissions → Allow/Deny
```

---

### Q: Explain JWT Authentication in ASP.NET Core

**Answer:**
**JWT** (JSON Web Token) = Stateless authentication token.

**JWT Structure:**
```
Header.Payload.Signature

eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.
eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.
SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
```

**Implementation:**

**Step 1: Install Package**
```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

**Step 2: Configure in appsettings.json**
```json
{
  "Jwt": {
    "Key": "your-super-secret-key-minimum-32-characters",
    "Issuer": "https://yourdomain.com",
    "Audience": "https://yourdomain.com",
    "ExpiryInMinutes": 60
  }
}
```

**Step 3: Configure Authentication**
```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();

app.UseAuthentication();  // Must be before UseAuthorization
app.UseAuthorization();
```

**Step 4: Generate Token**
```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class AuthService
{
    private readonly IConfiguration _configuration;
    
    public string GenerateToken(string userId, string email, string role)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(
                int.Parse(_configuration["Jwt:ExpiryInMinutes"])),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

**Step 5: Login Endpoint**
```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto login)
    {
        // Validate credentials (check database)
        if (login.Email == "user@example.com" && login.Password == "password")
        {
            var token = _authService.GenerateToken("123", login.Email, "Admin");
            return Ok(new { token });
        }
        
        return Unauthorized();
    }
}
```

**Step 6: Protect Endpoints**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]  // Requires authentication
public class ProductsController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]  // Public endpoint
    public IActionResult GetAll() { }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]  // Only Admin
    public IActionResult Create() { }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]  // Admin OR Manager
    public IActionResult Delete(int id) { }
}
```

**Testing:**
```bash
# 1. Login to get token
POST /api/auth/login
{
  "email": "user@example.com",
  "password": "password"
}

Response:
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}

# 2. Use token in requests
GET /api/products
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

### Q: What are Authorization Policies?

**Answer:**
**Policies** = Reusable authorization rules.

**Configuration:**
```csharp
builder.Services.AddAuthorization(options =>
{
    // Simple role policy
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireRole("Admin"));
    
    // Multiple roles
    options.AddPolicy("ManagementTeam", policy => 
        policy.RequireRole("Admin", "Manager"));
    
    // Claim-based
    options.AddPolicy("EmailVerified", policy => 
        policy.RequireClaim("EmailVerified", "true"));
    
    // Multiple requirements
    options.AddPolicy("SeniorEmployee", policy =>
    {
        policy.RequireRole("Employee");
        policy.RequireClaim("YearsOfService", "5", "10", "15");
    });
    
    // Custom requirement
    options.AddPolicy("MinimumAge", policy =>
        policy.Requirements.Add(new MinimumAgeRequirement(18)));
});
```

**Usage:**
```csharp
[Authorize(Policy = "AdminOnly")]
public class AdminController : ControllerBase { }

[HttpDelete("{id}")]
[Authorize(Policy = "SeniorEmployee")]
public IActionResult Delete(int id) { }
```

**Custom Authorization Handler:**
```csharp
// Requirement
public class MinimumAgeRequirement : IAuthorizationRequirement
{
    public int MinimumAge { get; }
    
    public MinimumAgeRequirement(int minimumAge)
    {
        MinimumAge = minimumAge;
    }
}

// Handler
public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        MinimumAgeRequirement requirement)
    {
        var ageClaim = context.User.FindFirst(c => c.Type == "Age");
        
        if (ageClaim != null && int.Parse(ageClaim.Value) >= requirement.MinimumAge)
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}

// Register
builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();
```

---

## 7. Entity Framework Core

### Q: What is Entity Framework Core? Explain approaches.

**Answer:**
**EF Core** = Object-Relational Mapper (ORM) for .NET

**3 Approaches:**

1. **Code First** (Most Common)
   - Write C# classes → Generate database

2. **Database First**
   - Existing database → Generate C# classes

3. **Model First**
   - Design model → Generate database and classes

**Code First Example:**
```csharp
// 1. Entity Class
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    
    // Navigation property
    public Category Category { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    // Navigation property
    public ICollection<Product> Products { get; set; }
}

// 2. DbContext
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API configuration
        modelBuilder.Entity<Product>()
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");
    }
}

// 3. Register in Program.cs
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

---

### Q: What are migrations? How to use them?

**Answer:**
**Migrations** = Version control for database schema.

**Commands:**
```bash
# 1. Add migration
dotnet ef migrations add InitialCreate

# 2. Update database
dotnet ef database update

# 3. Remove last migration
dotnet ef migrations remove

# 4. Generate SQL script
dotnet ef migrations script

# 5. List migrations
dotnet ef migrations list
```

**Migration Class:**
```csharp
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Products",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(maxLength: 100, nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
            });
    }
    
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Products");
    }
}
```

---

### Q: Explain LINQ queries in EF Core

**Answer:**

**Query Syntax vs Method Syntax:**
```csharp
// Query Syntax
var products = from p in _context.Products
               where p.Price > 100
               orderby p.Name
               select p;

// Method Syntax (Preferred)
var products = _context.Products
    .Where(p => p.Price > 100)
    .OrderBy(p => p.Name)
    .ToList();
```

**Common Operations:**
```csharp
public class ProductRepository
{
    private readonly AppDbContext _context;
    
    // 1. Get All
    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }
    
    // 2. Find by ID
    public async Task<Product> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
        // OR
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    // 3. Filter
    public async Task<List<Product>> GetExpensiveProductsAsync()
    {
        return await _context.Products
            .Where(p => p.Price > 1000)
            .ToListAsync();
    }
    
    // 4. Sorting
    public async Task<List<Product>> GetSortedAsync()
    {
        return await _context.Products
            .OrderBy(p => p.Name)
            .ThenByDescending(p => p.Price)
            .ToListAsync();
    }
    
    // 5. Paging
    public async Task<List<Product>> GetPagedAsync(int page, int pageSize)
    {
        return await _context.Products
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    // 6. Projection
    public async Task<List<ProductDto>> GetProductDtosAsync()
    {
        return await _context.Products
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            })
            .ToListAsync();
    }
    
    // 7. Join
    public async Task<List<ProductWithCategory>> GetWithCategoryAsync()
    {
        return await _context.Products
            .Include(p => p.Category)  // Eager loading
            .Select(p => new ProductWithCategory
            {
                ProductName = p.Name,
                CategoryName = p.Category.Name
            })
            .ToListAsync();
    }
    
    // 8. Count
    public async Task<int> GetCountAsync()
    {
        return await _context.Products.CountAsync();
    }
    
    // 9. Any/All
    public async Task<bool> HasExpensiveProductsAsync()
    {
        return await _context.Products.AnyAsync(p => p.Price > 1000);
    }
    
    // 10. Group By
    public async Task<List<CategoryStats>> GetCategoryStatsAsync()
    {
        return await _context.Products
            .GroupBy(p => p.CategoryId)
            .Select(g => new CategoryStats
            {
                CategoryId = g.Key,
                Count = g.Count(),
                AveragePrice = g.Average(p => p.Price)
            })
            .ToListAsync();
    }
}
```

---

### Q: Explain Eager Loading, Lazy Loading, and Explicit Loading

**Answer:**

**1. Eager Loading** - Load related data immediately
```csharp
// Include related data
var products = await _context.Products
    .Include(p => p.Category)  // Single level
    .ToListAsync();

// Multiple levels
var products = await _context.Products
    .Include(p => p.Category)
    .Include(p => p.Reviews)
        .ThenInclude(r => r.User)  // Nested include
    .ToListAsync();

// Multiple navigation properties
var products = await _context.Products
    .Include(p => p.Category)
    .Include(p => p.Supplier)
    .ToListAsync();
```

**2. Lazy Loading** - Load related data when accessed
```csharp
// Install: Microsoft.EntityFrameworkCore.Proxies

// Configure
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
           .UseLazyLoadingProxies());

// Entity must have virtual navigation properties
public class Product
{
    public int Id { get; set; }
    public virtual Category Category { get; set; }  // virtual keyword
}

// Usage
var product = await _context.Products.FindAsync(1);
var categoryName = product.Category.Name;  // Automatically loads Category
```

**3. Explicit Loading** - Manually load related data
```csharp
var product = await _context.Products.FindAsync(1);

// Load related Category
await _context.Entry(product)
    .Reference(p => p.Category)
    .LoadAsync();

// Load collection
await _context.Entry(product)
    .Collection(p => p.Reviews)
    .LoadAsync();

// Filter loaded data
await _context.Entry(product)
    .Collection(p => p.Reviews)
    .Query()
    .Where(r => r.Rating >= 4)
    .LoadAsync();
```

**Comparison:**

| Type | When Loaded | Performance | Use Case |
|------|-------------|-------------|----------|
| **Eager** | Query time | Good (1 query) | Known related data needed |
| **Lazy** | First access | Poor (N+1 problem) | Avoid in most cases |
| **Explicit** | Manual | Good | Conditional loading |

---

### Q: What is Repository Pattern? Implement it.

**Answer:**
**Repository Pattern** = Abstraction layer between business logic and data access.

**Benefits:**
- ✅ Separation of concerns
- ✅ Testability (mock repository)
- ✅ Centralized data access logic
- ✅ Reduced code duplication

**Implementation:**

```csharp
// 1. Generic Repository Interface
public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

// 2. Generic Repository Implementation
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
    
    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
    
    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    
    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<bool> ExistsAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        return entity != null;
    }
}

// 3. Specific Repository Interface
public interface IProductRepository : IRepository<Product>
{
    Task<List<Product>> GetExpensiveProductsAsync(decimal minPrice);
    Task<List<Product>> GetByCategoryAsync(int categoryId);
}

// 4. Specific Repository Implementation
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }
    
    public async Task<List<Product>> GetExpensiveProductsAsync(decimal minPrice)
    {
        return await _dbSet
            .Where(p => p.Price >= minPrice)
            .Include(p => p.Category)
            .ToListAsync();
    }
    
    public async Task<List<Product>> GetByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }
}

// 5. Register in Program.cs
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
// OR use generic registration
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// 6. Use in Service/Controller
public class ProductService
{
    private readonly IProductRepository _repository;
    
    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _repository.GetAllAsync();
    }
    
    public async Task<List<Product>> GetExpensiveProductsAsync()
    {
        return await _repository.GetExpensiveProductsAsync(1000);
    }
}
```

---

### Q: What is Unit of Work Pattern?

**Answer:**
**Unit of Work** = Manages transactions and coordinates saving changes across multiple repositories.

**Implementation:**
```csharp
// 1. Unit of Work Interface
public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    IOrderRepository Orders { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

// 2. Unit of Work Implementation
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction _transaction;
    
    public IProductRepository Products { get; }
    public ICategoryRepository Categories { get; }
    public IOrderRepository Orders { get; }
    
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Products = new ProductRepository(_context);
        Categories = new CategoryRepository(_context);
        Orders = new OrderRepository(_context);
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }
    
    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            await _transaction.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
    
    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

// 3. Register
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 4. Usage
public class OrderService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task CreateOrderAsync(Order order, List<OrderItem> items)
    {
        await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            // Add order
            await _unitOfWork.Orders.AddAsync(order);
            
            // Update product stock
            foreach (var item in items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                product.Stock -= item.Quantity;
                await _unitOfWork.Products.UpdateAsync(product);
            }
            
            // Commit all changes
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
```

---

## 8. API Versioning

### Q: How to implement API Versioning?

**Answer:**
See [API Versioning Interview Doc](#type-of-versioning-in-api-net-core---step-by-step-guide-to-achieve-that) section above for complete details.

**Quick Summary:**
```csharp
// Install
dotnet add package Microsoft.AspNetCore.Mvc.Versioning

// Configure
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// Controller
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase { }
```

---

## 9. Error Handling & Logging

### Q: How to implement global error handling?

**Answer:**

**Method 1: UseExceptionHandler Middleware**
```csharp
var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        
        var error = context.Features.Get<IExceptionHandlerFeature>();
        if (error != null)
        {
            var ex = error.Error;
            
            await context.Response.WriteAsJsonAsync(new
            {
                StatusCode = 500,
                Message = "Internal Server Error",
                Details = ex.Message
            });
        }
    });
});
```

**Method 2: Custom Exception Middleware**
```csharp
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var (statusCode, message) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            ValidationException => (StatusCodes.Status400BadRequest, exception.Message),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };
        
        context.Response.StatusCode = statusCode;
        
        await context.Response.WriteAsJsonAsync(new
        {
            StatusCode = statusCode,
            Message = message,
            Details = exception.Message
        });
    }
}

// Register
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

**Method 3: Problem Details (RFC 7807)**
```csharp
builder.Services.AddProblemDetails();

app.UseExceptionHandler();
app.UseStatusCodePages();

// Returns standardized error format:
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Product with ID 999 not found"
}
```

---

### Q: How to implement Logging?

**Answer:**

**Built-in Logging:**
```csharp
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    
    public ProductsController(ILogger<ProductsController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("Getting product with ID: {ProductId}", id);
        
        try
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", id);
                return NotFound();
            }
            
            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with ID {ProductId}", id);
            throw;
        }
    }
}
```

**Log Levels:**
```csharp
_logger.LogTrace("Trace message");       // Most detailed
_logger.LogDebug("Debug message");       // Debugging info
_logger.LogInformation("Info message");  // General info
_logger.LogWarning("Warning message");   // Warning
_logger.LogError(ex, "Error message");   // Error
_logger.LogCritical(ex, "Critical");     // Critical error
```

**appsettings.json Configuration:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

**Serilog (Popular Library):**
```csharp
// Install: Serilog.AspNetCore
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Rich structured logging
_logger.LogInformation("User {UserId} created order {OrderId}", userId, orderId);
```

---

## 10. Performance & Optimization

### Q: How to improve API performance?

**Answer:**

**1. Async/Await**
```csharp
// ❌ Synchronous - Blocks thread
public IActionResult Get()
{
    var products = _context.Products.ToList();
    return Ok(products);
}

// ✅ Asynchronous - Frees thread
public async Task<IActionResult> GetAsync()
{
    var products = await _context.Products.ToListAsync();
    return Ok(products);
}
```

**2. Pagination**
```csharp
[HttpGet]
public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
{
    var products = await _context.Products
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
        
    var totalCount = await _context.Products.CountAsync();
    
    return Ok(new
    {
        Data = products,
        Page = page,
        PageSize = pageSize,
        TotalCount = totalCount,
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
    });
}
```

**3. Response Caching**
```csharp
// Configure
builder.Services.AddResponseCaching();
app.UseResponseCaching();

// Use in controller
[HttpGet]
[ResponseCache(Duration = 60)]  // Cache for 60 seconds
public async Task<IActionResult> Get()
{
    return Ok(await _service.GetAllAsync());
}

// Different cache per query parameter
[ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "category" })]
public async Task<IActionResult> GetByCategory([FromQuery] string category)
{
    return Ok(await _service.GetByCategoryAsync(category));
}
```

**4. Memory Caching**
```csharp
// Configure
builder.Services.AddMemoryCache();

// Use in service
public class ProductService
{
    private readonly IMemoryCache _cache;
    private readonly IProductRepository _repository;
    
    public async Task<List<Product>> GetAllAsync()
    {
        const string cacheKey = "all_products";
        
        if (!_cache.TryGetValue(cacheKey, out List<Product> products))
        {
            products = await _repository.GetAllAsync();
            
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                
            _cache.Set(cacheKey, products, cacheOptions);
        }
        
        return products;
    }
}
```

**5. Compression**
```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
});

app.UseResponseCompression();
```

**6. AsNoTracking for Read-Only Queries**
```csharp
// ❌ Tracked (slower for read-only)
var products = await _context.Products.ToListAsync();

// ✅ Not tracked (faster for read-only)
var products = await _context.Products
    .AsNoTracking()
    .ToListAsync();
```

**7. Select Only Required Data**
```csharp
// ❌ Gets all columns
var products = await _context.Products.ToListAsync();

// ✅ Gets only needed columns
var products = await _context.Products
    .Select(p => new ProductDto
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price
    })
    .ToListAsync();
```

**8. Avoid N+1 Query Problem**
```csharp
// ❌ N+1 queries
var products = await _context.Products.ToListAsync();
foreach (var product in products)
{
    var category = await _context.Categories.FindAsync(product.CategoryId); // N queries
}

// ✅ Single query with Include
var products = await _context.Products
    .Include(p => p.Category)
    .ToListAsync();
```

---

### Q: How to implement Rate Limiting?

**Answer:**
```csharp
// .NET 7+ built-in rate limiting
builder.Services.AddRateLimiter(options =>
{
    // Fixed window
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 10;
        opt.QueueLimit = 0;
    });
    
    // Sliding window
    options.AddSlidingWindowLimiter("sliding", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.SegmentsPerWindow = 6;
        opt.PermitLimit = 100;
    });
    
    // Concurrency (max concurrent requests)
    options.AddConcurrencyLimiter("concurrency", opt =>
    {
        opt.PermitLimit = 5;
        opt.QueueLimit = 10;
    });
});

app.UseRateLimiter();

// Apply to endpoint
[HttpGet]
[EnableRateLimiting("fixed")]
public IActionResult Get()
{
    return Ok("Rate limited endpoint");
}
```

---

## 11. Design Patterns & Architecture

### Q: What is Clean Architecture?

**Answer:**
**Clean Architecture** = Layered architecture with dependency inversion.

**Layers (Inside → Outside):**
```
Domain (Entities)
    ↓
Application (Use Cases, Interfaces)
    ↓
Infrastructure (Data Access, External Services)
    ↓
Presentation (API, Controllers)
```

**Project Structure:**
```
Solution
├── Domain (Core)
│   ├── Entities
│   └── Interfaces
├── Application
│   ├── DTOs
│   ├── Services
│   └── Validators
├── Infrastructure
│   ├── Data (DbContext, Repositories)
│   └── Services (External APIs)
└── API (Web)
    └── Controllers
```

**Example:**
```csharp
// 1. Domain Layer - Entities
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// 2. Domain Layer - Interface
public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product> GetByIdAsync(int id);
}

// 3. Application Layer - Service
public class ProductService
{
    private readonly IProductRepository _repository;
    
    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<List<ProductDto>> GetAllProductsAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price
        }).ToList();
    }
}

// 4. Infrastructure Layer - Repository
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    
    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }
}

// 5. Presentation Layer - Controller
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var products = await _service.GetAllProductsAsync();
        return Ok(products);
    }
}
```

---

### Q: What is CQRS Pattern?

**Answer:**
**CQRS** = Command Query Responsibility Segregation

**Concept:**
- **Commands** = Write operations (Create, Update, Delete)
- **Queries** = Read operations (Get, List)

**Why use CQRS?**
- ✅ Separation of concerns
- ✅ Different models for read/write
- ✅ Scalability (separate read/write databases)
- ✅ Performance optimization

**Implementation with MediatR:**
```csharp
// Install: MediatR

// 1. Query
public class GetProductByIdQuery : IRequest<ProductDto>
{
    public int Id { get; set; }
}

// 2. Query Handler
public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _repository;
    
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id);
        return new ProductDto { Id = product.Id, Name = product.Name };
    }
}

// 3. Command
public class CreateProductCommand : IRequest<int>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// 4. Command Handler
public class CreateProductHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IProductRepository _repository;
    
    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price
        };
        
        await _repository.AddAsync(product);
        return product.Id;
    }
}

// 5. Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// 6. Controller
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetProductByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }
}
```

---

## 12. Testing

### Q: How to write unit tests for Web API?

**Answer:**
```csharp
// Install: xUnit, Moq, FluentAssertions

// Test Class
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly ProductService _service;
    
    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _service = new ProductService(_repositoryMock.Object);
    }
    
    [Fact]
    public async Task GetAllAsync_ReturnsProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Price = 100 },
            new Product { Id = 2, Name = "Product 2", Price = 200 }
        };
        
        _repositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(products);
        
        // Act
        var result = await _service.GetAllAsync();
        
        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Product 1");
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }
    
    [Fact]
    public async Task GetByIdAsync_ProductExists_ReturnsProduct()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Product 1", Price = 100 };
        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(product);
        
        // Act
        var result = await _service.GetByIdAsync(1);
        
        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Product 1");
    }
    
    [Fact]
    public async Task GetByIdAsync_ProductNotFound_ReturnsNull()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Product)null);
        
        // Act
        var result = await _service.GetByIdAsync(999);
        
        // Assert
        result.Should().BeNull();
    }
}
```

---

### Q: How to write integration tests?

**Answer:**
```csharp
// Install: Microsoft.AspNetCore.Mvc.Testing

// Test Class
public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public ProductsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/products");
        
        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Create_ValidProduct_ReturnsCreated()
    {
        // Arrange
        var product = new { Name = "Test Product", Price = 100 };
        var content = new StringContent(
            JsonSerializer.Serialize(product), 
            Encoding.UTF8, 
            "application/json");
        
        // Act
        var response = await _client.PostAsync("/api/products", content);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
```

---

## 13. Advanced Topics

### Q: What is CORS? How to enable it?

**Answer:**
**CORS** (Cross-Origin Resource Sharing) = Allows requests from different domains.

**Configuration:**
```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Add CORS policy
builder.Services.AddCors(options =>
{
    // Allow specific origin
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://example.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
    
    // Allow any origin (Development only!)
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
    
    // Multiple origins
    options.AddPolicy("AllowMultiple", policy =>
    {
        policy.WithOrigins("https://example1.com", "https://example2.com")
              .AllowAnyHeader()
              .WithMethods("GET", "POST");
    });
});

var app = builder.Build();

// 2. Use CORS (before UseAuthorization)
app.UseCors("AllowSpecificOrigin");

// OR apply to specific endpoint
app.MapControllers().RequireCors("AllowSpecificOrigin");

// OR apply to specific controller/action
[EnableCors("AllowSpecificOrigin")]
public class ProductsController : ControllerBase { }
```

---

### Q: What is Swagger/OpenAPI? How to configure it?

**Answer:**
**Swagger** = API documentation and testing tool.

**Configuration:**
```csharp
// Install: Swashbuckle.AspNetCore

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "API for managing products",
        Contact = new OpenApiContact
        {
            Name = "Support",
            Email = "support@example.com"
        }
    });
    
    // Add JWT authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    
    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        options.RoutePrefix = string.Empty; // Swagger at root
    });
}
```

**Controller Documentation:**
```csharp
/// <summary>
/// Get all products
/// </summary>
/// <returns>List of products</returns>
/// <response code="200">Returns list of products</response>
[HttpGet]
[ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
public async Task<IActionResult> GetAll()
{
    return Ok(await _service.GetAllAsync());
}

/// <summary>
/// Create a new product
/// </summary>
/// <param name="product">Product details</param>
/// <returns>Created product</returns>
/// <response code="201">Product created successfully</response>
/// <response code="400">Invalid product data</response>
[HttpPost]
[ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> Create([FromBody] CreateProductDto product)
{
    var created = await _service.CreateAsync(product);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
}
```

---

### Q: What is Background Service? How to implement it?

**Answer:**
**Background Service** = Long-running task that runs in the background.

**Implementation:**
```csharp
// 1. Create Background Service
public class EmailBackgroundService : BackgroundService
{
    private readonly ILogger<EmailBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    
    public EmailBackgroundService(
        ILogger<EmailBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email Background Service started");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                
                await emailService.SendPendingEmailsAsync();
                
                // Wait 1 minute before next check
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Email Background Service");
            }
        }
        
        _logger.LogInformation("Email Background Service stopped");
    }
}

// 2. Register
builder.Services.AddHostedService<EmailBackgroundService>();
```

---

## 🎯 Interview Tips

### Must Know Topics (Priority Order):
1. ✅ Web API Basics (REST, HTTP methods, Status codes)
2. ✅ Dependency Injection (3 lifetimes)
3. ✅ Entity Framework Core (LINQ, Migrations)
4. ✅ Authentication & Authorization (JWT)
5. ✅ Middleware Pipeline
6. ✅ Error Handling & Logging
7. ✅ Repository Pattern
8. ✅ API Versioning
9. ✅ Performance Optimization
10. ✅ Clean Architecture / CQRS

### Common Interview Questions:
- Explain REST principles
- Difference between AddScoped, AddTransient, AddSingleton
- What is middleware?
- How does JWT authentication work?
- What is the difference between Authentication and Authorization?
- Explain Repository Pattern
- How to handle errors globally?
- What is async/await?
- How to optimize API performance?
- Explain Clean Architecture

### Quick Answers Template:
1. **Define** the concept
2. **Explain** why it's used
3. **Give** a real-world example
4. **Show** code implementation
5. **Mention** best practices

---

**Good luck with your interview! 🚀**

This covers 95% of .NET Core Web API interview questions. Practice the code examples and you'll be well-prepared!