// This file contains code for Program.
using HotelCore.Infrastructure;
using HotelCore.Api.Middlewares;
using HotelCore.Application;
using HotelCore.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using HotelCore.Infrastructure.Persistence;
using HotelCore.ServiceDefaults;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);


builder.ConfigureLogging();

builder.AddServiceDefaults();


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


builder.Services.ConfigureAuthentication(builder.Configuration);


builder.Services.AddHttpContextAccessor();

//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyMethod()
//              .AllowAnyHeader();
//    });
//});

var app = builder.Build();

app.MapDefaultEndpoints();


app.UseOpenApi();

app.UseExceptionHandler();
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

//app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}


await app.ResetTestDataAsync();  
await app.SeedRolesAsync();
await app.SeedAdminUserAsync();
await app.SeedTestUsersAsync();
await app.SeedCleaningWorkersAsync();
await app.SeedRoomsAsync();
await app.SeedRestaurantDataAsync();
await app.SeedDemoReservationAsync();

app.Run();