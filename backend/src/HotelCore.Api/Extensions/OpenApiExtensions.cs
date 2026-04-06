using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace HotelCore.Api.Extensions;

public static class OpenApiExtensions
{
    public static void ConfigureOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "HotelCore API",
                    Version = "v1"
                };

                return Task.CompletedTask;
            });

            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });
    }

    public static void UseOpenApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.Title = "HotelCore API Reference";
                options.Theme = ScalarTheme.Mars;
            });
            app.MapGet("/", () => Results.Redirect("/scalar"));
        }
    }

    private sealed class BearerSecuritySchemeTransformer(
        IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            var authSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

            if (!authSchemes.Any(s => s.Name == "Bearer"))
                return;

            var securityScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                In = ParameterLocation.Header,
                Name = "Authorization",
                BearerFormat = "JWT",
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes["Bearer"] = securityScheme;

            var schemeRef = new OpenApiSecuritySchemeReference("Bearer", document);

            var requirement = new OpenApiSecurityRequirement
            {
                [schemeRef] = []
            };

            foreach (var operation in document.Paths.Values.SelectMany(p => p.Operations.Values))
            {
                operation.Security ??= [];
                operation.Security.Add(requirement);
            }
        }
    }
}
