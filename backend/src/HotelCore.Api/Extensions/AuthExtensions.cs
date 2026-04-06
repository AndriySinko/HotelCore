using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace HotelCore.Api.Extensions;

public static class AuthExtensions
{
    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(IdentityConstants.ExternalScheme, options =>
            {
                options.Cookie.Name = "Workers.External";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddGoogle(options =>
            {
                options.ClientId = configuration["GoogleAuth:ClientId"] ?? "google-client-id";
                options.ClientSecret = configuration["GoogleAuth:ClientSecret"] ?? "google-client-secret";
                options.SignInScheme = IdentityConstants.ExternalScheme;
                
                options.SaveTokens = true;
                
                options.Events.OnRemoteFailure = context =>
                {
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("GoogleAuth");
                    
                    logger.LogError("Google Auth Remote Failure: {Error}", context.Failure?.Message);
                    context.Response.Redirect("/login?error=remote_failure");
                    context.HandleResponse();
                    return Task.CompletedTask;
                };
            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger("AuthDebug");

                        var roles = context.Principal?.Claims
                            .Where(c => c.Type == ClaimTypes.Role)
                            .Select(c => c.Value)
                            .ToArray() ?? Array.Empty<string>();

                        var legacyRoles = context.Principal?.Claims
                            .Where(c => c.Type == "role")
                            .Select(c => c.Value)
                            .ToArray() ?? Array.Empty<string>();

                        var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                                     ?? context.Principal?.FindFirstValue("uid")
                                     ?? context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);

                        logger.LogInformation(
                            "Auth ok. UserId={UserId}; Roles={Roles}; LegacyRoles={LegacyRoles}",
                            userId ?? "unknown",
                            roles.Length == 0 ? "none" : string.Join(",", roles),
                            legacyRoles.Length == 0 ? "none" : string.Join(",", legacyRoles));

                        if (logger.IsEnabled(LogLevel.Debug) && context.Principal is not null)
                        {
                            var claimsDump = string.Join(
                                "; ",
                                context.Principal.Claims.Select(c => $"{c.Type}={c.Value}"));
                            logger.LogDebug("Claims: {Claims}", claimsDump);
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger("AuthDebug");

                        logger.LogError(context.Exception, "Auth failed");
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger("AuthDebug");

                        var userId = context.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                                     ?? context.HttpContext.User?.FindFirstValue("uid")
                                     ?? context.HttpContext.User?.FindFirstValue(JwtRegisteredClaimNames.Sub);

                        logger.LogWarning("Forbidden. UserId={UserId}", userId ?? "unknown");
                        return Task.CompletedTask;
                    }
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"]!)),
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = "name"
                };
            });

        services.AddAuthorization();
    }
}
