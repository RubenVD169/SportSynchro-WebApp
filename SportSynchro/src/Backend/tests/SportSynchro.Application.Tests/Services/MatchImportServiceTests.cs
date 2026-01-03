using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DomainMatch = SportSynchro.Domain.Entities.Match;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;
using Moq;
using Xunit;

using SportSynchro.Application.Services;
using SportSynchro.Application.Interfaces.External;
using SportSynchro.Application.Interfaces.Repositories;

using SportSynchro.Domain.Entities;
using SportSynchro.Domain.ValueObjects;

using SportSynchro.External.TheSportsDb.Contracts.Models.Matches;




namespace SportSynchro.Application.Tests.Services;

public sealed class MatchImportServiceTests
{
    private const int LeagueId = 10;
    private const int LeagueExternalId = 99;

    private static Season CreateSeasonWithId(int seasonId, string key = "2023-2024", bool isCurrent = true)
    {
        var season = new Season(
            leagueId: LeagueId,
            key: SeasonKey.Create(key),
            isCurrent: isCurrent);

        SetPrivateProperty(season, nameof(Season.Id), seasonId);
        return season;
    }

    private static Dictionary<int, int> CreateTeamLookup()
        => new()
        {
            { 100, 1 }, // external team -> internal team
            { 200, 2 }
        };

    private static TheSportsDbMatchDto CreateDto(
        int eventId,
        int homeExternalTeamId,
        int awayExternalTeamId,
        string timestamp,
        string status)
        => new()
        {
            IdEvent = eventId,
            IdHomeTeam = homeExternalTeamId,
            IdAwayTeam = awayExternalTeamId,
            StrTimestamp = timestamp,
            StrStatus = status
        };

    private static void SetPrivateProperty<T>(T instance, string propertyName, object value)
    {
        var prop = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        prop.Should().NotBeNull($"Property '{propertyName}' should exist on {typeof(T).Name}.");

        var setter = prop!.GetSetMethod(nonPublic: true);
        setter.Should().NotBeNull($"Property '{propertyName}' should have a setter (non-public is fine).");

        setter!.Invoke(instance, new[] { value });
    }

    // No teams in season -> early return null
    [Fact]
    public async Task ImportMatchesForSeasonAsync_NoTeamsInSeason_ReturnsNull()
    {
        // Arrange
        var season = CreateSeasonWithId(seasonId: 1);

        var seasonTeamRepo = new Mock<ISeasonTeamRepository>(MockBehavior.Strict);
        seasonTeamRepo
            .Setup(x => x.GetTeamLookupForSeasonAsync(season.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, int>());

        var sportsDb = new Mock<ITheSportsDbRepository>(MockBehavior.Strict);
        var matchRepo = new Mock<IMatchRepository>(MockBehavior.Strict);

        var service = new MatchImportService(
            sportsDb.Object,
            seasonTeamRepo.Object,
            matchRepo.Object);

        // Act
        DateTime? result = await service.ImportMatchesForSeasonAsync(season, LeagueExternalId);

        // Assert
        result.Should().BeNull();

        seasonTeamRepo.VerifyAll();
        sportsDb.VerifyNoOtherCalls();
        matchRepo.VerifyNoOtherCalls();
    }

    // API returns no matches -> early return null
    [Fact]
    public async Task ImportMatchesForSeasonAsync_NoApiMatches_ReturnsNull()
    {
        // Arrange
        var season = CreateSeasonWithId(seasonId: 1);

        var seasonTeamRepo = new Mock<ISeasonTeamRepository>(MockBehavior.Strict);
        seasonTeamRepo
            .Setup(x => x.GetTeamLookupForSeasonAsync(season.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTeamLookup());

        var sportsDb = new Mock<ITheSportsDbRepository>(MockBehavior.Strict);
        sportsDb
            .Setup(x => x.GetMatchesByLeagueAndSeasonAsync(
                LeagueExternalId,
                season.Key.Value,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TheSportsDbMatchDto>());

        var matchRepo = new Mock<IMatchRepository>(MockBehavior.Strict);

        var service = new MatchImportService(
            sportsDb.Object,
            seasonTeamRepo.Object,
            matchRepo.Object);

        // Act
        DateTime? result = await service.ImportMatchesForSeasonAsync(season, LeagueExternalId);

        // Assert
        result.Should().BeNull();

        seasonTeamRepo.VerifyAll();
        sportsDb.VerifyAll();
        matchRepo.VerifyNoOtherCalls();
    }

    // All matches already exist -> no adds, SaveChanges called, returns null
    [Fact]
    public async Task ImportMatchesForSeasonAsync_AllMatchesExist_SkipsAll_ReturnsNull()
    {
        // Arrange
        var season = CreateSeasonWithId(seasonId: 1);

        var dto = CreateDto(
            eventId: 1,
            homeExternalTeamId: 100,
            awayExternalTeamId: 200,
            timestamp: "2024-02-01T15:00:00Z",
            status: "NS");

        var seasonTeamRepo = new Mock<ISeasonTeamRepository>(MockBehavior.Strict);
        seasonTeamRepo
            .Setup(x => x.GetTeamLookupForSeasonAsync(season.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTeamLookup());

        var sportsDb = new Mock<ITheSportsDbRepository>(MockBehavior.Strict);
        sportsDb
            .Setup(x => x.GetMatchesByLeagueAndSeasonAsync(
                LeagueExternalId,
                season.Key.Value,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { dto });

        var matchRepo = new Mock<IMatchRepository>(MockBehavior.Strict);
        matchRepo
            .Setup(x => x.GetExistingExternalIdsForSeasonAsync(
                season.Id,
                It.IsAny<HashSet<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<int> { 1 });

        matchRepo
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new MatchImportService(
            sportsDb.Object,
            seasonTeamRepo.Object,
            matchRepo.Object);

        // Act
        DateTime? result = await service.ImportMatchesForSeasonAsync(season, LeagueExternalId);

        // Assert
        result.Should().BeNull();

        matchRepo.Verify(x => x.AddAsync(It.IsAny<DomainMatch>(), It.IsAny<CancellationToken>()), Times.Never);
        matchRepo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        seasonTeamRepo.VerifyAll();
        sportsDb.VerifyAll();
        matchRepo.VerifyAll();
    }

    // Unknown team -> mapping fails -> skipped, SaveChanges called, returns null
    [Fact]
    public async Task ImportMatchesForSeasonAsync_UnknownTeam_SkipsMatch_ReturnsNull()
    {
        // Arrange
        var season = CreateSeasonWithId(seasonId: 1);

        // home team not in lookup (999)
        var dto = CreateDto(
            eventId: 1,
            homeExternalTeamId: 999,
            awayExternalTeamId: 200,
            timestamp: "2024-02-01T15:00:00Z",
            status: "NS");

        var seasonTeamRepo = new Mock<ISeasonTeamRepository>(MockBehavior.Strict);
        seasonTeamRepo
            .Setup(x => x.GetTeamLookupForSeasonAsync(season.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTeamLookup());

        var sportsDb = new Mock<ITheSportsDbRepository>(MockBehavior.Strict);
        sportsDb
            .Setup(x => x.GetMatchesByLeagueAndSeasonAsync(
                LeagueExternalId,
                season.Key.Value,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { dto });

        var matchRepo = new Mock<IMatchRepository>(MockBehavior.Strict);
        matchRepo
            .Setup(x => x.GetExistingExternalIdsForSeasonAsync(
                season.Id,
                It.IsAny<HashSet<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<int>());

        matchRepo
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new MatchImportService(
            sportsDb.Object,
            seasonTeamRepo.Object,
            matchRepo.Object);

        // Act
        DateTime? result = await service.ImportMatchesForSeasonAsync(season, LeagueExternalId);

        // Assert
        result.Should().BeNull();

        matchRepo.Verify(x => x.AddAsync(It.IsAny<DomainMatch>(), It.IsAny<CancellationToken>()), Times.Never);
        matchRepo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        seasonTeamRepo.VerifyAll();
        sportsDb.VerifyAll();
        matchRepo.VerifyAll();
    }

    // Invalid timestamp -> mapping fails -> skipped, SaveChanges called, returns null
    [Fact]
    public async Task ImportMatchesForSeasonAsync_InvalidTimestamp_SkipsMatch_ReturnsNull()
    {
        // Arrange
        var season = CreateSeasonWithId(seasonId: 1);

        var dto = CreateDto(
            eventId: 1,
            homeExternalTeamId: 100,
            awayExternalTeamId: 200,
            timestamp: "not-a-date",
            status: "NS");

        var seasonTeamRepo = new Mock<ISeasonTeamRepository>(MockBehavior.Strict);
        seasonTeamRepo
            .Setup(x => x.GetTeamLookupForSeasonAsync(season.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTeamLookup());

        var sportsDb = new Mock<ITheSportsDbRepository>(MockBehavior.Strict);
        sportsDb
            .Setup(x => x.GetMatchesByLeagueAndSeasonAsync(
                LeagueExternalId,
                season.Key.Value,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { dto });

        var matchRepo = new Mock<IMatchRepository>(MockBehavior.Strict);
        matchRepo
            .Setup(x => x.GetExistingExternalIdsForSeasonAsync(
                season.Id,
                It.IsAny<HashSet<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<int>());

        matchRepo
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new MatchImportService(
            sportsDb.Object,
            seasonTeamRepo.Object,
            matchRepo.Object);

        // Act
        DateTime? result = await service.ImportMatchesForSeasonAsync(season, LeagueExternalId);

        // Assert
        result.Should().BeNull();

        matchRepo.Verify(x => x.AddAsync(It.IsAny<DomainMatch>(), It.IsAny<CancellationToken>()), Times.Never);
        matchRepo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        seasonTeamRepo.VerifyAll();
        sportsDb.VerifyAll();
        matchRepo.VerifyAll();
    }

    /* ============================================================
       Multiple matches, mixed validity, latest returned
       - one existing -> skipped
       - one valid earlier -> imported
       - one valid later -> imported (latest)
       - one invalid team -> skipped
       ============================================================ */
    [Fact]
    public async Task ImportMatchesForSeasonAsync_MixedMatches_ImportsValidAndReturnsLatestUtc()
    {
        // Arrange
        var season = CreateSeasonWithId(seasonId: 1);

        var existing = CreateDto(
            eventId: 10,
            homeExternalTeamId: 100,
            awayExternalTeamId: 200,
            timestamp: "2024-01-01T10:00:00Z",
            status: "NS");

        var validEarly = CreateDto(
            eventId: 11,
            homeExternalTeamId: 100,
            awayExternalTeamId: 200,
            timestamp: "2024-02-01T10:00:00Z",
            status: "FT");

        var validLate = CreateDto(
            eventId: 12,
            homeExternalTeamId: 100,
            awayExternalTeamId: 200,
            timestamp: "2024-03-01T10:00:00Z",
            status: "1H");

        var invalidTeam = CreateDto(
            eventId: 13,
            homeExternalTeamId: 999,
            awayExternalTeamId: 200,
            timestamp: "2024-04-01T10:00:00Z",
            status: "NS");

        var apiMatches = new[] { existing, validEarly, validLate, invalidTeam };

        var seasonTeamRepo = new Mock<ISeasonTeamRepository>(MockBehavior.Strict);
        seasonTeamRepo
            .Setup(x => x.GetTeamLookupForSeasonAsync(season.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTeamLookup());

        var sportsDb = new Mock<ITheSportsDbRepository>(MockBehavior.Strict);
        sportsDb
            .Setup(x => x.GetMatchesByLeagueAndSeasonAsync(
                LeagueExternalId,
                season.Key.Value,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiMatches);

        var matchRepo = new Mock<IMatchRepository>(MockBehavior.Strict);
        matchRepo
            .Setup(x => x.GetExistingExternalIdsForSeasonAsync(
                season.Id,
                It.IsAny<HashSet<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<int> { 10 });

        matchRepo
            .Setup(x => x.AddAsync(It.IsAny<DomainMatch>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        matchRepo
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new MatchImportService(
            sportsDb.Object,
            seasonTeamRepo.Object,
            matchRepo.Object);

        // Act
        DateTime? result = await service.ImportMatchesForSeasonAsync(season, LeagueExternalId);

        // Assert
        result.Should().Be(DateTime.Parse("2024-03-01T10:00:00Z").ToUniversalTime());

        matchRepo.Verify(
            x => x.AddAsync(
                It.Is<DomainMatch>(m =>
                    m.SeasonId == season.Id &&
                    (m.ExternalId == 11 || m.ExternalId == 12) &&
                    m.HomeTeamId == 1 &&
                    m.AwayTeamId == 2 &&
                    (m.Status.Value == "Finished" || m.Status.Value == "In Progress")),
                It.IsAny<CancellationToken>()),
            Times.Exactly(2));

        matchRepo.Verify(x => x.AddAsync(It.Is<DomainMatch>(m => m.ExternalId == 10), It.IsAny<CancellationToken>()), Times.Never);
        matchRepo.Verify(x => x.AddAsync(It.Is<DomainMatch>(m => m.ExternalId == 13), It.IsAny<CancellationToken>()), Times.Never);

        matchRepo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        seasonTeamRepo.VerifyAll();
        sportsDb.VerifyAll();
        matchRepo.VerifyAll();
    }
}
