using Duende.IdentityServer;
using SportSynchro.IdentityServer.Data;
using SportSynchro.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Duende.IdentityServer.EntityFramework.DbContexts;
using SportSynchro.IdentityServer.Options;
using Microsoft.Extensions.Options;

namespace SportSynchro.IdentityServer;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        builder.Services.Configure<DatabaseOptions>(
            builder.Configuration.GetSection(nameof(DatabaseOptions)));

        builder.Services.Configure<FrontendOptions>(
            builder.Configuration.GetSection(nameof(FrontendOptions)));

        builder.Services.Configure<CorsOptions>(
            builder.Configuration.GetSection(nameof(CorsOptions)));



        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                DatabaseOptions dbOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
                options.UseSqlServer(dbOptions.DefaultConnection);
            });

        builder.Services.AddDbContext<ConfigurationDbContext>((sp, options) =>
            {
                DatabaseOptions dbOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;

                options.UseSqlServer(
                    dbOptions.DefaultConnection,
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

        builder.Services.AddCors();

        builder.Services.AddControllers();

        builder.Services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
            // in-memory stores, keys, clients and scopes
            // .AddInMemoryIdentityResources(Config.IdentityResources)
            // .AddInMemoryApiScopes(Config.ApiScopes)
            // .AddInMemoryClients(Config.Clients)
            .AddConfigurationStore()
            .AddAspNetIdentity<ApplicationUser>()
            .AddProfileService<ProfileService>();


        builder.Services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                // register your IdentityServer with Google at https://console.developers.google.com
                // enable the Google+ API
                // set the redirect URI to https://localhost:5001/signin-google
                options.ClientId = "copy client ID from Google here";
                options.ClientSecret = "copy client secret from Google here";
            });

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
    
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseCors(policyBuilder =>
            {
                CorsOptions corsOptions = app.Services
                    .GetRequiredService<IOptions<CorsOptions>>()
                    .Value;

                policyBuilder
                    .WithOrigins(corsOptions.AllowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();
        app.MapControllers();

        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}