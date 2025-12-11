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
        options.Authority = "https://localhost:5001";
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


