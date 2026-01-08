using SportSynchro.Api.Options;

namespace SportSynchro.Api.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddSportSynchroCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        CorsOptions corsOptions = configuration
            .GetSection(nameof(CorsOptions))
            .Get<CorsOptions>()!;

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .WithOrigins(corsOptions.AllowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}
