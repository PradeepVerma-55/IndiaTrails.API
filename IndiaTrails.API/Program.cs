using FluentValidation.AspNetCore;
using IndiaTrails.API.Data;
using IndiaTrails.API.Mappings;
using IndiaTrails.API.Models.Validators;
using IndiaTrails.API.Repositories;
using IndiaTrails.API.Repositories.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Serilog Configuration

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/IndiaTrailsAPI-.log", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();


builder.Services.AddApiVersioning(options =>
{
    // Specify the default API version
    options.DefaultApiVersion = new ApiVersion(1, 0);

    // Assume default version when not specified
    options.AssumeDefaultVersionWhenUnspecified = true;

    // Report API versions in response headers
    options.ReportApiVersions = true;

    // Choose versioning method (pick one or combine):

    // 1. URL Path Versioning
    options.ApiVersionReader = new UrlSegmentApiVersionReader();

    // 2. Query String Versioning
    // options.ApiVersionReader = new QueryStringApiVersionReader("api-version");

    // 3. Header Versioning
    // options.ApiVersionReader = new HeaderApiVersionReader("X-API-Version");

    // 4. Combine multiple methods
    // options.ApiVersionReader = ApiVersionReader.Combine(
    //     new UrlSegmentApiVersionReader(),
    //     new QueryStringApiVersionReader("api-version"),
    //     new HeaderApiVersionReader("X-API-Version")
    // );
});

// Add API Explorer for Swagger support
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    var provider = builder.Services.BuildServiceProvider()
        .GetRequiredService<IApiVersionDescriptionProvider>();

    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerDoc(
            description.GroupName,
            new OpenApiInfo
            {
                Title = $"My API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = description.IsDeprecated
                    ? "This API version has been deprecated."
                    : ""
            });
    }
});

//Automapper Configuration
builder.Services.AddAutoMapper(typeof(Program).Assembly);

//Sql Server Dependency Injection

builder.Services.AddDbContext<IndiaTrailsDBContext>(options =>
                                                        options.UseSqlServer(builder.Configuration.GetConnectionString("IndiaTrailsDBConnectionString"))
                                                   );

//Fluent Validation Configuration
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AddRegionRequestDtoValidator>());
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AddWalkRequestDtoValidator>());

builder.Services.AddScoped<IRegionRepository, SqlServerRegionRepository>();
builder.Services.AddScoped<IWalkRepository, SqlServerWalkRepository>();
builder.Services.AddScoped<IAuthRepository, SqlServerAuthRepository>();

// JWT Authentication Configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
