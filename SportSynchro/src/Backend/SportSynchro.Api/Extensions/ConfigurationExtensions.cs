using Azure.Identity;

namespace SportSynchro.Api.Extensions;

public static class ConfigurationExtensions
{
  public static IConfigurationBuilder AddSportSynchroKeyVault(
      this IConfigurationBuilder builder,
      IHostEnvironment environment)
  {
    if (!environment.IsProduction()) return builder;
    string? keyVaultName = builder.Build()["KeyVaultName"];
    if (!string.IsNullOrWhiteSpace(keyVaultName))
    {
      builder.AddAzureKeyVault(
        new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential());
    }

    return builder;
  }
}
