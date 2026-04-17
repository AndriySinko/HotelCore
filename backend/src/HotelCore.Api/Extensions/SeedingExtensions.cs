// This file contains code for SeedingExtensions.
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HotelCore.Domain.Entities.Reception;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Enums;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Api.Extensions;

public static class SeedingExtensions
{
    
    
    
    
    public static async Task ResetTestDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await db.Database.ExecuteSqlRawAsync("DELETE FROM shifts");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM work_schedules");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM cleaning_tasks");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM payments");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM reservations");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM \"AspNetUsers\" WHERE role = 0");
    }

    public static async Task SeedRolesAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        foreach (var role in Enum.GetValues<UserRole>())
        {
            var roleName = role.ToString();
            if (role == UserRole.Guest) continue;
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }
    }

    public static async Task SeedAdminUserAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        const string adminEmail = "admin@hotelcore.com";
        const string adminPassword = "Admin123!";

        var existing = await userManager.FindByEmailAsync(adminEmail);
        if (existing != null)
        {
            
            existing.Role = UserRole.Administrator;
            await userManager.UpdateAsync(existing);
            var currentRoles = await userManager.GetRolesAsync(existing);
            if (!currentRoles.Contains(UserRole.Administrator.ToString()))
            {
                await userManager.RemoveFromRolesAsync(existing, currentRoles);
                await userManager.AddToRoleAsync(existing, UserRole.Administrator.ToString());
            }
            return;
        }

        var admin = new User
        {
            Email = adminEmail,
            UserName = adminEmail,
            Role = UserRole.Administrator,
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(admin, adminPassword);
        await userManager.AddToRoleAsync(admin, UserRole.Administrator.ToString());
    }

    public static async Task SeedTestUsersAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var users = new[]
        {
            (Email: "receptionist@hotelcore.com", FirstName: "Jane",  LastName: "Smith",    Role: UserRole.Receptionist),
            (Email: "cleaner@hotelcore.com",      FirstName: "Alice", LastName: "Clean",    Role: UserRole.CleaningWorker),
            (Email: "supervisor@hotelcore.com",   FirstName: "Bob",   LastName: "Super",    Role: UserRole.Supervisor),
            (Email: "manager@hotelcore.com",      FirstName: "Mike",  LastName: "Manager",  Role: UserRole.HotelManager),
            (Email: "guest@hotelcore.com",        FirstName: "John",  LastName: "Guest",    Role: UserRole.Guest),
        };

        foreach (var (email, firstName, lastName, role) in users)
        {
            if (await userManager.FindByEmailAsync(email) != null) continue;

            var user = new User
            {
                Email = email,
                UserName = email,
                Role = role,
                EmailConfirmed = true,
            };
            var result = await userManager.CreateAsync(user, "Admin123!");
            if (result.Succeeded && role != UserRole.Guest)
                await userManager.AddToRoleAsync(user, role.ToString());
        }
    }

    public static async Task SeedCleaningWorkersAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var workers = new[]
        {
            (Email: "worker1@hotelcore.com", DisplayName: "Maria Lopez",   Position: "Room Attendant"),
            (Email: "worker2@hotelcore.com", DisplayName: "James Brown",   Position: "Room Attendant"),
            (Email: "worker3@hotelcore.com", DisplayName: "Sofia Nowak",   Position: "Laundry Attendant"),
            (Email: "worker4@hotelcore.com", DisplayName: "Dawit Bekele",  Position: "Floor Supervisor"),
            (Email: "worker5@hotelcore.com", DisplayName: "Priya Sharma",  Position: "Room Attendant"),
        };

        foreach (var (email, displayName, position) in workers)
        {
            if (await userManager.FindByEmailAsync(email) != null) continue;

            var userName = displayName.ToLowerInvariant().Replace(" ", ".");

            var worker = new StaffMember
            {
                Email                = email,
                UserName             = userName,
                Role                 = UserRole.CleaningWorker,
                EmailConfirmed       = true,
                Department           = "Housekeeping",
                Position             = position,
                ContractHoursPerWeek = 40,
                HireDate             = new DateTime(2025, 3, 1),
            };
            var result = await userManager.CreateAsync(worker, "Admin123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(worker, UserRole.CleaningWorker.ToString());
        }
    }

    public static async Task SeedRoomsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (await db.Rooms.AnyAsync())
            return;

        var rooms = new List<Room>();

        
        for (int i = 1; i <= 14; i++)
            rooms.Add(new Room { RoomNumber = $"1{i:D2}", RoomType = RoomType.Single,  Floor = 1, PricePerNight = 80m });

        
        for (int i = 1; i <= 14; i++)
            rooms.Add(new Room { RoomNumber = $"2{i:D2}", RoomType = RoomType.Double,  Floor = 2, PricePerNight = 120m });

        
        for (int i = 1; i <= 8; i++)
            rooms.Add(new Room { RoomNumber = $"3{i:D2}", RoomType = RoomType.Family,  Floor = 3, PricePerNight = 160m });

        
        for (int i = 1; i <= 8; i++)
            rooms.Add(new Room { RoomNumber = $"4{i:D2}", RoomType = RoomType.Deluxe,  Floor = 4, PricePerNight = 200m });

        
        for (int i = 1; i <= 6; i++)
            rooms.Add(new Room { RoomNumber = $"5{i:D2}", RoomType = RoomType.Suite,   Floor = 5, PricePerNight = 350m });

        db.Rooms.AddRange(rooms);
        await db.SaveChangesAsync();
    }
}
