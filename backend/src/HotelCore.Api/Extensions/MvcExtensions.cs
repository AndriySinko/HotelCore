using Microsoft.AspNetCore.Mvc;
using HotelCore.Api.Models;

namespace HotelCore.Api.Extensions;

public static class MvcExtensions
{
    public static IServiceCollection ConfigureMvc(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var apiError = ApiError.Validation("One or more validation errors occurred.", errors);

                return new BadRequestObjectResult(ApiResult.Failure(apiError));
            };
        });

        return services;
    }
}
