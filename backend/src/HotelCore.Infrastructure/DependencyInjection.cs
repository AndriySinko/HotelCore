using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Storage;
using HotelCore.Application.Identity;
using HotelCore.Domain.Entities.Users;
using HotelCore.Infrastructure.Identity;
using HotelCore.Infrastructure.Persistence;
using HotelCore.Infrastructure.Repositories;
using HotelCore.Infrastructure.Services;
using HotelCore.Infrastructure.Storage;

namespace HotelCore.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<ApplicationDbContext>("DefaultConnection",
            configureDbContextOptions: options =>
            {
                options.UseSnakeCaseNamingConvention();
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

        builder.AddRedisClient("redis");
        builder.AddStorageServices();
    }

    private static void AddStorageServices(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<CloudflareR2Options>(
            builder.Configuration.GetSection(CloudflareR2Options.SectionName));

        builder.Services.AddSingleton<IFileStorageService, CloudflareR2StorageService>();
    }
}
