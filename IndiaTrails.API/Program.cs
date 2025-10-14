using System.Text;
using FluentValidation.AspNetCore;
using IndiaTrails.API.Data;
using IndiaTrails.API.Mappings;
using IndiaTrails.API.Models.Validators;
using IndiaTrails.API.Repositories;
using IndiaTrails.API.Repositories.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Serilog Configuration

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/IndiaTrailsAPI-.log", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
