namespace SportSynchro.Application.Interfaces.Blob
{
    public interface ISchedulePdfService
    {
        Task<byte[]> GetSchedulePdfAsync(int leagueId, CancellationToken cancellationToken);        
    }
}