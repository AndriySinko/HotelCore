using HotelCore.Infrastructure;
using HotelCore.Api.Middlewares;
using HotelCore.Application;
using HotelCore.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using HotelCore.Infrastructure.Persistence;
using HotelCore.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Configure Logging
builder.ConfigureLogging();

builder.AddServiceDefaults();

// Add Services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.ConfigureVersioning();
builder.Services.ConfigureOpenApi();
builder.Services.ConfigureMvc();
builder.Services.AddApplication();
builder.AddInfrastructure();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Configure Auth
builder.Services.ConfigureAuthentication(builder.Configuration);

// Register authorization handlers
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure Pipeline
app.UseOpenApi();

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply Migrations Automatically (Development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Seed Data
await app.SeedRolesAsync();

app.Run();