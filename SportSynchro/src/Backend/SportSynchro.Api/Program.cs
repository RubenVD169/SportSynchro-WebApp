using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SportSynchro.Api.Options;
using SportSynchro.Infrastructure.Persistence;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection(nameof(DatabaseOptions)));

builder.Services.Configure<AuthenticationOptions>(
    builder.Configuration.GetSection(nameof(AuthenticationOptions)));

builder.Services.Configure<CorsOptions>(
    builder.Configuration.GetSection(nameof(CorsOptions)));

builder.Services.AddDbContext<SportSynchroDbContext>((sp, options) =>
{
    DatabaseOptions dbOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
    options.UseSqlServer(dbOptions.ConnectionString);
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add authentication and authorization
AuthenticationOptions authOptions = builder.Configuration
    .GetSection(nameof(AuthenticationOptions))
    .Get<AuthenticationOptions>()!;

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = authOptions.Authority;
        options.TokenValidationParameters.ValidateAudience = false;
    });



builder.Services.AddAuthorizationBuilder()
    .AddPolicy("ReadPolicy", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("scope",
                "sportsynchro.api.read");
        })
    .AddPolicy("WritePolicy", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("scope",
                "sportsynchro.api.write");
        });

CorsOptions corsOptions = builder.Configuration
    .GetSection(nameof(CorsOptions))
    .Get<CorsOptions>()!;

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(corsOptions.AllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();


app.Run();


