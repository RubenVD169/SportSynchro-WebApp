using SportSynchro.IdentityServer.Data;
using SportSynchro.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Duende.IdentityServer.EntityFramework.DbContexts;
using SportSynchro.IdentityServer.Options;
using Microsoft.Extensions.Options;
using Azure.Identity;

namespace SportSynchro.IdentityServer;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddAzureKeyVault(
    new Uri("https://sportsynchro-keyvault.vault.azure.net/"),
    new DefaultAzureCredential()
);
        builder.Services.AddRazorPages();

        builder.Services.Configure<DatabaseOptions>(
            builder.Configuration.GetSection(DatabaseOptions.SectionName));

        builder.Services.Configure<FrontendOptions>(
            builder.Configuration.GetSection(nameof(FrontendOptions)));

        builder.Services.Configure<CorsOptions>(
            builder.Configuration.GetSection(nameof(CorsOptions)));

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                DatabaseOptions dbOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
                options.UseSqlServer(dbOptions.ConnectionString);
            });

        builder.Services.AddDbContext<ConfigurationDbContext>((sp, options) =>
            {
                DatabaseOptions dbOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;

                options.UseSqlServer(
                    dbOptions.ConnectionString,
                    sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name)
                );
            });

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>
            (options =>
                {
                    options.User.RequireUniqueEmail = true;
                })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("FrontendCors", policy =>
                {
                    CorsOptions corsOptions = builder.Configuration
                        .GetSection(nameof(CorsOptions))
                        .Get<CorsOptions>() ?? throw new InvalidOperationException("CorsOptions not configured");

                    policy
                        .WithOrigins(corsOptions.AllowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        builder.Services.AddControllers();

        builder.Services.AddMemoryCache();

        DatabaseOptions dbOptions =
            builder.Configuration
                .GetSection(DatabaseOptions.SectionName)
                .Get<DatabaseOptions>()
            ?? throw new InvalidOperationException("DatabaseOptions not configured");

        builder.Services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;
            })
            .AddConfigurationStore()
            .AddAspNetIdentity<ApplicationUser>()
            .AddProfileService<ProfileService>()
            .AddInMemoryCaching()
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(
                        dbOptions.ConnectionString,
                        sql => sql.MigrationsAssembly(
                            typeof(Program).Assembly.GetName().Name
                        )
                    );
            });

        builder.Services.AddAuthentication();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors("FrontendCors");
        app.UseIdentityServer();
        app.UseAuthorization();
        app.MapControllers();

        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}