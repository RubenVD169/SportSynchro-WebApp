using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

using SportSynchro.Application.Services;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Models.Matches;

namespace SportSynchro.Application.Tests.Services;

public sealed class MatchServiceTests
{
    [Fact]
    public async Task GetRecentMatchesByLeagueIdAsync_ReturnsMatchesFromRepository()
    {
        // Arrange
        const int leagueId = 10;

        var expected = new List<MatchModel>
        {
            new MatchModel(
                Id: 1,
                LeagueName: "Premier League",
                MatchDate: DateTime.UtcNow,
                HomeTeam: "Team A",
                AwayTeam: "Team B",
                HomeScore: 2,
                AwayScore: 1,
                Status: "Finished"),

            new MatchModel(
                Id: 2,
                LeagueName: "Premier League",
                MatchDate: DateTime.UtcNow,
                HomeTeam: "Team C",
                AwayTeam: "Team D",
                HomeScore: 0,
                AwayScore: 0,
                Status: "Finished")
        };

        var repo = new Mock<IMatchRepository>(MockBehavior.Strict);
        repo
            .Setup(x => x.GetRecentFinishedMatchesByLeagueIdAsync(
                leagueId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var service = new MatchService(repo.Object);

        // Act
        IReadOnlyList<MatchModel> result =
            await service.GetRecentMatchesByLeagueIdAsync(leagueId, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(expected);

        repo.VerifyAll();
    }

    [Fact]
    public async Task GetRecentMatchesByLeagueIdAsync_NoMatches_ReturnsEmptyList()
    {
        // Arrange
        const int leagueId = 99;

        var repo = new Mock<IMatchRepository>(MockBehavior.Strict);
        repo
            .Setup(x => x.GetRecentFinishedMatchesByLeagueIdAsync(
                leagueId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MatchModel>());

        var service = new MatchService(repo.Object);

        // Act
        IReadOnlyList<MatchModel> result =
            await service.GetRecentMatchesByLeagueIdAsync(leagueId, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();

        repo.VerifyAll();
    }
}
