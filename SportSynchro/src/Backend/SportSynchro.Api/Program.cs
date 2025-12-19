using Microsoft.EntityFrameworkCore;
using SportSynchro.Infrastructure.Persistence;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SportSynchroDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add authentication and authorization
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Authentication:Authority"];
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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? throw new InvalidOperationException("No allowed origins configured"))
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
app.UseAuthentication();
app.UseAuthorization();


app.Run();


