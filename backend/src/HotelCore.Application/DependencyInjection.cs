// This file contains code for DependencyInjection.
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using HotelCore.Application.Common.Behaviors;

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

        return services;
    }
}
