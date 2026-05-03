// application entry point — wires up all services and starts the HTTP server
// uses the extension methods in the Extensions/ folder to keep this file readable
using HotelCore.Infrastructure;
using HotelCore.Api.Middlewares;
using HotelCore.Application;
using HotelCore.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using HotelCore.Infrastructure.Persistence;
using HotelCore.ServiceDefaults;

// required for Npgsql to handle DateTime without timezone info correctly
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// structured logging via OpenTelemetry
builder.ConfigureLogging();

// Aspire service defaults — health checks, telemetry, etc.
builder.AddServiceDefaults();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // serialize enums as strings in JSON responses instead of numbers — more readable for the frontend
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.ConfigureVersioning();
builder.Services.ConfigureOpenApi();
builder.Services.ConfigureMvc();
// registers MediatR handlers and FluentValidation validators from the Application layer
builder.Services.AddApplication();
// registers EF Core, Identity, repositories, email and QR code services
builder.AddInfrastructure();

// global exception handler converts domain exceptions to structured error responses
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// JWT authentication — secret and settings come from appsettings.json
builder.Services.ConfigureAuthentication(builder.Configuration);

// needed so controllers can read the current user from the HTTP context
builder.Services.AddHttpContextAccessor();

// allow requests from the React frontend running on a different port in development
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

// Swagger UI — available in all environments for demo purposes
app.UseOpenApi();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// run pending EF migrations automatically in development so devs dont have to run dotnet ef manually
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

// seed initial data on every startup — safe to run repeatedly, checks before inserting
await app.ResetTestDataAsync();
await app.SeedRolesAsync();
await app.SeedAdminUserAsync();
await app.SeedTestUsersAsync();
await app.SeedCleaningWorkersAsync();
await app.SeedRoomsAsync();

app.Run();
