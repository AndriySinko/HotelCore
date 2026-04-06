using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using HotelCore.Application.Common.Authorization;
using HotelCore.Application.Common.Behaviors;
using HotelCore.Application.Common.Interfaces.Orders;
using HotelCore.Application.EmailVerification.Interfaces;
using HotelCore.Application.EmailVerification.Services;

namespace HotelCore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);
        services.AddSingleton<IVerificationCodeGenerator, VerificationCodeGenerator>();
        services.AddSingleton<IOrderAccessPolicy, OrderAccessPolicy>();

        return services;
    }
}
