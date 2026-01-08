using SportSynchro.Api.Extensions;
using SportSynchro.Api.Workers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddSportSynchroKeyVault(builder.Environment);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddExternalClients(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddSportSynchroAuthentication(builder.Configuration, builder.Environment);
builder.Services.AddSportSynchroAuthorization();
builder.Services.AddSportSynchroCors(builder.Configuration);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHostedService<SportsSeedingWorker>();
}

WebApplication app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
