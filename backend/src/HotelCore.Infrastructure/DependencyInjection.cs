// This file contains code for DependencyInjection.
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Cleaning;
using HotelCore.Application.Common.Interfaces.Reception;
using HotelCore.Application.Common.Interfaces.StaffManagement;
using HotelCore.Application.Common.Interfaces.Storage;
using HotelCore.Application.Identity;
using HotelCore.Domain.Entities.Users;
using HotelCore.Infrastructure.Identity;
using HotelCore.Infrastructure.Persistence;
using HotelCore.Infrastructure.Repositories;
using HotelCore.Infrastructure.Services;
using HotelCore.Infrastructure.Storage;
using Microsoft.Extensions.Logging;

namespace HotelCore.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            options.UseSnakeCaseNamingConvention();
            options.EnableDetailedErrors();
            options.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        });

        builder.Services.AddIdentityCore<User>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddScoped<IIdentityService, IdentityService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        builder.Services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

        
        builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
        builder.Services.AddScoped<IRoomRepository, RoomRepository>();
        builder.Services.AddScoped<IGuestRepository, GuestRepository>();

        
        builder.Services.AddScoped<IWorkScheduleRepository, WorkScheduleRepository>();
        builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
        builder.Services.AddScoped<IStaffRepository, StaffRepository>();

        
        builder.Services.AddScoped<ICleaningTaskRepository, CleaningTaskRepository>();

        
        builder.Services.AddScoped<IQrCodeService, QrCodeService>();

        var resendApiKey = builder.Configuration["Resend:ApiKey"];
        if (!string.IsNullOrWhiteSpace(resendApiKey) && resendApiKey != "REPLACE_WITH_YOUR_RESEND_API_KEY")
        {
            builder.Services.AddHttpClient("Resend", client =>
            {
                client.BaseAddress = new Uri("https://api.resend.com/");
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", resendApiKey);
            });
            builder.Services.AddScoped<IEmailService, ResendEmailService>();
        }
        else
        {
            builder.Services.AddScoped<IEmailService, ConsoleEmailService>();
        }

        var redisConnection = builder.Configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnection))
        {
            builder.Services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisConnection));
        }

        builder.AddStorageServices();
    }

    private static void AddStorageServices(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<CloudflareR2Options>(
            builder.Configuration.GetSection(CloudflareR2Options.SectionName));

        builder.Services.AddSingleton<IFileStorageService, CloudflareR2StorageService>();
    }
}
