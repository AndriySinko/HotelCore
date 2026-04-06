using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Categories;
using HotelCore.Application.Common.Interfaces.Images;
using HotelCore.Application.Common.Interfaces.Orders;
using HotelCore.Application.Common.Interfaces.WorkRequests;
using HotelCore.Application.Common.Interfaces.Storage;
using HotelCore.Application.Common.Interfaces.WorkersPortfolio;
using HotelCore.Application.EmailVerification.Interfaces;
using HotelCore.Application.EmailVerification.Options;
using HotelCore.Application.Identity;
using HotelCore.Application.Users;
using HotelCore.Domain.Entities.Users;
using HotelCore.Infrastructure.Caching;
using HotelCore.Infrastructure.Identity;
using HotelCore.Infrastructure.Images;
using HotelCore.Infrastructure.Notifications;
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
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddScoped<IUserEmailConfirmationService, UserEmailConfirmationService>();

        builder.Services.AddScoped<IEntityHistoryService, EntityHistoryService>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IWorkRequestRepository, WorkRequestRepository>();
        builder.Services.AddScoped<IImageGroupRepository, ImageGroupRepository>();
        builder.Services.AddScoped<IWorkerPortfolioItemRepository, WorkerPortfolioItemRepository>();
        builder.Services.AddSingleton<IOrdersTemporaryAccessStore, OrdersTemporaryAccessStore>();
        builder.Services.Configure<EmailVerificationOptions>(
            builder.Configuration.GetSection("EmailVerification"));
        builder.Services.Configure<NotificationServiceOptions>(
            builder.Configuration.GetSection("NotificationService"));

        builder.AddRedisClient("redis");
        builder.Services.AddSingleton<ICategoryCache, CategoryCache>();
        builder.Services.AddSingleton<IEmailVerificationStore, RedisEmailVerificationStore>();

        builder.Services.AddHttpClient<IVerificationNotificationPublisher, HttpVerificationNotificationPublisher>(
            (sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<NotificationServiceOptions>>().Value;
                var baseUrl = string.IsNullOrWhiteSpace(options.BaseUrl)
                    ? "http://notificationservice-api"
                    : options.BaseUrl;

                client.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
            });

        builder.Services.AddSingleton<IEventProducer, Messaging.KafkaProducer>();
        builder.AddStorageServices();
    }

    private static void AddStorageServices(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<CloudflareR2Options>(
            builder.Configuration.GetSection(CloudflareR2Options.SectionName));

        builder.Services.AddSingleton<IFileStorageService, CloudflareR2StorageService>();
        builder.Services.AddSingleton<IImageProcessor, ImageSharpProcessor>();
        builder.Services.AddScoped<ICategoryIconService, CategoryIconService>();
        builder.Services.AddScoped<IImageService, ImageService>();
    }
}
