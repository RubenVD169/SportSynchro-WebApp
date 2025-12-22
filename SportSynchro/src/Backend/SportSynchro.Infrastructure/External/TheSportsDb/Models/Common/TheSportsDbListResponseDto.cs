namespace SportSynchro.Infrastructure.External.TheSportsDb.Models.Common;

public sealed class TheSportsDbListResponseDto<T>
{
    public List<T>? Data { get; set; }
}
