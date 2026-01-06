using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

using SportSynchro.Application.Services;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Models.Matches;

namespace SportSynchro.Application.Tests.Services;

public sealed class MatchServiceTests
{
    [Fact]
    public async Task GetScheduledMatchesByLeagueIdAsync_ReturnsMatchesFromRepository()
    {
        // Arrange
        const int leagueId = 10;

        var expected = new List<MatchModel>
        {
            new MatchModel(
                Id: 1,
                LeagueName: "Premier League",
                MatchDate: DateTime.UtcNow.AddDays(1),
                HomeTeam: "Team A",
                AwayTeam: "Team B",
                HomeScore: 0,
                AwayScore: 0,
                Status: "Scheduled"),

            new MatchModel(
                Id: 2,
                LeagueName: "Premier League",
                MatchDate: DateTime.UtcNow.AddDays(2),
                HomeTeam: "Team C",
                AwayTeam: "Team D",
                HomeScore: 0,
                AwayScore: 0,
                Status: "Scheduled")
        };

        var repo = new Mock<IMatchRepository>(MockBehavior.Strict);
        repo
            .Setup(x => x.GetScheduledMatchesByLeagueIdAsync(
                leagueId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new MatchService(repo.Object, cache);

        // Act
        IReadOnlyList<MatchModel> result =
            await service.GetScheduledMatchesByLeagueIdAsync(leagueId, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(expected);

        repo.VerifyAll();
    }

    [Fact]
    public async Task GetRecentFinishedMatchesForVisibleLeaguesBySportIdAsync_CachesResults()
    {
        // Arrange
        const int sportId = 5;

        var matches = new List<MatchModel>
        {
            new MatchModel(
                Id: 1,
                LeagueName: "Premier League",
                MatchDate: DateTime.UtcNow,
                HomeTeam: "Team A",
                AwayTeam: "Team B",
                HomeScore: 2,
                AwayScore: 1,
                Status: "Finished")
        };

        var repo = new Mock<IMatchRepository>(MockBehavior.Strict);
        repo
            .Setup(x => x.GetRecentFinishedMatchesForVisibleLeaguesBySportIdAsync(
                sportId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(matches);

        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new MatchService(repo.Object, cache);

        // Act
        IReadOnlyList<MatchModel> result =
            await service.GetRecentFinishedMatchesForVisibleLeaguesBySportIdAsync(sportId, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(matches);
        repo.VerifyAll();
    }
}
