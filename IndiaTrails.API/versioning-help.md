# API Versioning - Interview Prep Guide

## 🎯 Quick Definition
**API Versioning** = Managing multiple versions of your API to allow updates without breaking existing clients.

---

## 📝 4 Types of Versioning

| Type | Example | When to Use |
|------|---------|-------------|
| **1. URL Path** ⭐ | `/api/v1/products` | **Most Common** - Easy to test |
| **2. Query String** | `/api/products?api-version=1.0` | Internal APIs |
| **3. Header** | `X-API-Version: 1.0` | Advanced scenarios |
| **4. Media Type** | `Accept: application/vnd.v1+json` | Strict REST APIs |

**Interview Tip:** Always mention **URL Path** as the recommended approach!

---

## ⚙️ Quick Implementation

### 1. Install Package
```bash
dotnet add package Microsoft.AspNetCore.Mvc.Versioning
```

### 2. Configure in Program.cs
```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});
```

### 3. Create Controller
```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() 
    {
        return Ok("Version 1.0");
    }
}
```

---

## 💡 Common Interview Questions

### Q1: Why do we need API versioning?
**Answer:**
- ✅ Support old clients while adding new features
- ✅ Avoid breaking existing applications
- ✅ Allow gradual migration
- ✅ Maintain backward compatibility

### Q2: What's the difference between v1 and v2?
**Answer:** Increment version for **breaking changes**:
- Removing endpoints
- Renaming fields
- Changing data types
- Modifying authentication

### Q3: Which versioning method is best?
**Answer:** **URL Path** (`/api/v1/products`)
- **Why?** Clear, explicit, easy to test, visible in browser

### Q4: How to deprecate old versions?
**Answer:**
```csharp
[ApiVersion("1.0", Deprecated = true)]  // Mark as deprecated
[ApiVersion("2.0")]                      // Current version
```
- Give **6-12 months notice**
- Communicate via release notes
- Return deprecation warning in headers

### Q5: Can same endpoint support multiple versions?
**Answer:** Yes!
```csharp
[HttpGet("{id}")]
[MapToApiVersion("1.0")]
[MapToApiVersion("2.0")]
public IActionResult GetById(int id) { }
```

---

## 🔑 Key Configuration Options

```csharp
builder.Services.AddApiVersioning(options =>
{
    // If client doesn't specify version, use this
    options.DefaultApiVersion = new ApiVersion(1, 0);
    
    // Use default when version not specified
    options.AssumeDefaultVersionWhenUnspecified = true;
    
    // Add version info to response headers
    options.ReportApiVersions = true;
    
    // Choose versioning type
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});
```

---

## 📊 Comparison Table

| Feature | URL Path | Query String | Header |
|---------|----------|--------------|--------|
| Visibility | ✅ High | ⚠️ Medium | ❌ Hidden |
| Easy to Test | ✅ Yes | ✅ Yes | ⚠️ Needs tools |
| SEO Friendly | ✅ Yes | ⚠️ Maybe | ❌ No |
| Caching | ✅ Easy | ⚠️ Complex | ⚠️ Complex |
| **Recommended** | **✅ YES** | ❌ No | ❌ No |

---

## 🎨 Real-World Example

### Scenario: Product API Evolution

**Version 1.0** - Basic Product
```csharp
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { id = 1, name = "Product" });
    }
}
```

**Version 2.0** - Enhanced Product (Breaking Change)
```csharp
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsV2Controller : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        // Changed structure - requires new version
        return Ok(new 
        { 
            productId = 1,        // Renamed: id → productId
            productName = "Product", // Renamed: name → productName
            price = 100,          // New field
            category = "Electronics" // New field
        });
    }
}
```

**URLs:**
- V1: `GET /api/v1/products` → Returns basic structure
- V2: `GET /api/v2/products` → Returns enhanced structure

---

## ✅ Best Practices (Must Know!)

1. **Always version from day 1** - Start with v1
2. **Use major versions only** - v1, v2, v3 (not v1.1, v1.2)
3. **Support max 2-3 versions** - Don't maintain too many
4. **Deprecate gracefully** - Give 6-12 months notice
5. **Document changes** - Clear migration guides
6. **Use URL Path** - Most developer-friendly

---

## 🚨 Common Mistakes to Avoid

| ❌ Don't | ✅ Do |
|---------|-------|
| Change v1 behavior | Create v2 for breaking changes |
| Support 5+ versions | Support max 2-3 versions |
| Remove versions suddenly | Deprecate with warning period |
| Use date versioning (2024-01-15) | Use semantic versioning (v1, v2) |
| Version every minor change | Only version breaking changes |

---

## 🧪 Testing (Show You Know!)

```bash
# Test different versions
curl https://api.example.com/api/v1/products
curl https://api.example.com/api/v2/products

# Check supported versions (Response headers)
curl -I https://api.example.com/api/v1/products
# Returns:
# api-supported-versions: 1.0, 2.0
# api-deprecated-versions: 1.0
```

---

## 📱 Flutter Integration (Your Advantage!)

```dart
class ApiService {
  final String baseUrl = 'https://api.example.com';
  final String version = 'v2'; // Easy version switching
  
  Future<List<Product>> getProducts() async {
    final response = await dio.get('$baseUrl/api/$version/products');
    return (response.data as List)
        .map((json) => Product.fromJson(json))
        .toList();
  }
}
```

**Interview Bonus:** "I integrate versioned APIs in my Flutter apps, making it easy to switch between API versions during development and testing."

---

## 🎯 One-Liner Summary

"API Versioning allows multiple API versions to coexist using URL paths like `/api/v1/products`, enabling backward compatibility while introducing breaking changes in newer versions."

---

## 💭 Interview Response Template

**When asked about API Versioning, say:**

> "API versioning is essential for maintaining backward compatibility. I prefer **URL Path versioning** like `/api/v1/products` because it's explicit and easy to test. 
>
> I use the **Microsoft.AspNetCore.Mvc.Versioning** package, configure it in Program.cs with `AddApiVersioning`, and create versioned controllers with `[ApiVersion("1.0")]` attribute. 
>
> For breaking changes like renaming fields or removing endpoints, I increment to v2. I also mark old versions as deprecated to give clients time to migrate. 
>
> In my Flutter apps, I easily switch between API versions during development."

---

## 📌 Remember These 3 Things

1. **Package:** `Microsoft.AspNetCore.Mvc.Versioning`
2. **Best Method:** URL Path (`/api/v1/products`)
3. **When to Version:** Breaking changes only
