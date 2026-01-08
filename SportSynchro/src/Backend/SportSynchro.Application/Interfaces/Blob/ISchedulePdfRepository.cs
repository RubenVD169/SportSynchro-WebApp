namespace SportSynchro.Application.Interfaces.Blob
{
    public interface ISchedulePdfRepository
    {
        Task WriteAsync(byte[] content, int leagueId, int seasonId); 
        Task<byte[]> ReadAsync(int leagueId, int seasonId);
    }
}