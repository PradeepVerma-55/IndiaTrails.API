using FluentValidation.AspNetCore;
using IndiaTrails.API.Data;
using IndiaTrails.API.Mappings;
using IndiaTrails.API.Models.Validators;
using IndiaTrails.API.Repositories;
using IndiaTrails.API.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IRegionRepository,SqlServerRegionRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
