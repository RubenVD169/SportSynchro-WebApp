using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

using SportSynchro.Application.Services;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Interfaces.External;
using SportSynchro.Application.Models.Leagues;

using SportSynchro.Domain.Entities;
using SportSynchro.Domain.ValueObjects;

namespace SportSynchro.Application.Tests.Services;

public sealed class LeagueServiceTests
{
    private const int LeagueId = 1;
    private const int LeagueExternalId = 100;
    private const int SportId = 10;

    // Helpers
    private static League CreateLeague(bool isVisible)
    {
        var league = new League(
            externalId: LeagueExternalId,
            name: LeagueName.Create("Premier League"),
            sportId: SportId,
            isVisible: isVisible);

        SetPrivateProperty(league, nameof(League.Id), LeagueId);
        return league;
    }

    private static Season CreateSeason(int id, string key, bool isCurrent)
    {
        var season = new Season(
            leagueId: LeagueId,
            key: SeasonKey.Create(key),
            isCurrent: isCurrent);

        SetPrivateProperty(season, nameof(Season.Id), id);
        return season;
    }

    private static void SetPrivateProperty<T>(T instance, string propertyName, object value)
    {
        var prop = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        prop.Should().NotBeNull();

        var setter = prop!.GetSetMethod(nonPublic: true);
        setter.Should().NotBeNull();

        setter!.Invoke(instance, [value]);
    }

    // League not found -> returns false
    [Fact]
    public async Task SetLeagueVisibilityAsync_LeagueNotFound_ReturnsFalse()
    {
        // Arrange
        var leagueRepo = new Mock<ILeagueRepository>(MockBehavior.Strict);
        leagueRepo
            .Setup(x => x.GetByIdAsync(LeagueId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((League?)null);

        var seasonRepo = new Mock<ISeasonRepository>(MockBehavior.Strict);
        var teamImport = new Mock<ITeamImportService>(MockBehavior.Strict);
        var matchImport = new Mock<IMatchImportService>(MockBehavior.Strict);
        var sportsDb = new Mock<ITheSportsDbRepository>(MockBehavior.Strict);
        var cache = new MemoryCache(new MemoryCacheOptions());

        var service = new LeagueService(
            leagueRepo.Object,
            seasonRepo.Object,
            teamImport.Object,
            matchImport.Object,
            sportsDb.Object,
            cache);

        // Act
        bool result =
            await service.SetLeagueVisibilityAsync(LeagueId, true);

        // Assert
        result.Should().BeFalse();
        leagueRepo.VerifyAll();
        seasonRepo.VerifyNoOtherCalls();
        teamImport.VerifyNoOtherCalls();
        matchImport.VerifyNoOtherCalls();
        sportsDb.VerifyNoOtherCalls();
    }

    // Set visibility to false -> no imports
    [Fact]
    public async Task SetLeagueVisibilityAsync_SetInvisible_SavesAndStops()
    {
        // Arrange
        var league = CreateLeague(isVisible: true);

        var leagueRepo = new Mock<ILeagueRepository>(MockBehavior.Strict);
        leagueRepo
            .Setup(x => x.GetByIdAsync(LeagueId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(league);

        leagueRepo
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var seasonRepo = new Mock<ISeasonRepository>(MockBehavior.Strict);
        var teamImport = new Mock<ITeamImportService>(MockBehavior.Strict);
        var matchImport = new Mock<IMatchImportService>(MockBehavior.Strict);
        var sportsDb = new Mock<ITheSportsDbRepository>(MockBehavior.Strict);
        var cache = new MemoryCache(new MemoryCacheOptions());

        var service = new LeagueService(
            leagueRepo.Object,
            seasonRepo.Object,
            teamImport.Object,
            matchImport.Object,
            sportsDb.Object,
            cache);

        // Act
        bool result =
            await service.SetLeagueVisibilityAsync(LeagueId, false);

        // Assert
        result.Should().BeTrue();
        league.IsVisible.Should().BeFalse();

        leagueRepo.VerifyAll();
        seasonRepo.VerifyNoOtherCalls();
        teamImport.VerifyNoOtherCalls();
        matchImport.VerifyNoOtherCalls();
        sportsDb.VerifyNoOtherCalls();
    }

    // Visible + no current season -> creates new season
    [Fact]
    public async Task SetLeagueVisibilityAsync_NoCurrentSeason_CreatesSeasonAndImports()
    {
        // Arrange
        var league = CreateLeague(isVisible: false);

        var leagueRepo = new Mock<ILeagueRepository>(MockBehavior.Strict);
        leagueRepo
            .Setup(x => x.GetByIdAsync(LeagueId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(league);

        leagueRepo
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var seasonRepo = new Mock<ISeasonRepository>(MockBehavior.Strict);
        seasonRepo
            .Setup(x => x.GetCurrentForLeagueAsync(LeagueId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Season?)null);

        seasonRepo
            .Setup(x => x.AddAsync(It.IsAny<Season>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        seasonRepo
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sportsDb = new Mock<ITheSportsDbRepository>(MockBehavior.Strict);
        sportsDb
            .Setup(x => x.GetSeasonsByLeagueAsync(
                LeagueExternalId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "2022", "2023" });

        var teamImport = new Mock<ITeamImportService>(MockBehavior.Strict);
        teamImport
            .Setup(x => x.ImportTeamsForSeasonAsync(
                It.IsAny<Season>(),
                LeagueExternalId,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var matchImport = new Mock<IMatchImportService>(MockBehavior.Strict);
        matchImport
            .Setup(x => x.ImportMatchesForSeasonAsync(
                It.IsAny<Season>(),
                LeagueExternalId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((DateTime?)null);

        var cache = new Mock<IMemoryCache>(MockBehavior.Loose);
        var service = new LeagueService(
            leagueRepo.Object,
            seasonRepo.Object,
            teamImport.Object,
            matchImport.Object,
            sportsDb.Object,
            cache.Object);

        // Act
        bool result =
            await service.SetLeagueVisibilityAsync(LeagueId, true);

        // Assert
        result.Should().BeTrue();
        league.IsVisible.Should().BeTrue();

        leagueRepo.VerifyAll();
        seasonRepo.VerifyAll();
        teamImport.VerifyAll();
        matchImport.VerifyAll();
        sportsDb.VerifyAll();
    }

    // Matches imported -> marks season and saves
    [Fact]
    public async Task SetLeagueVisibilityAsync_MatchesImported_MarksSeason()
    {
        // Arrange
        var league = CreateLeague(isVisible: false);
        var season = CreateSeason(id: 5, key: "2023", isCurrent: true);

        var leagueRepo = new Mock<ILeagueRepository>(MockBehavior.Strict);
        leagueRepo
            .Setup(x => x.GetByIdAsync(LeagueId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(league);

        leagueRepo
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var seasonRepo = new Mock<ISeasonRepository>(MockBehavior.Strict);
        seasonRepo
            .Setup(x => x.GetCurrentForLeagueAsync(LeagueId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);

        seasonRepo
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sportsDb = new Mock<ITheSportsDbRepository>(MockBehavior.Strict);
        sportsDb
            .Setup(x => x.GetSeasonsByLeagueAsync(
                LeagueExternalId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "2023" });

        var teamImport = new Mock<ITeamImportService>(MockBehavior.Strict);
        teamImport
            .Setup(x => x.ImportTeamsForSeasonAsync(
                season,
                LeagueExternalId,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var matchImport = new Mock<IMatchImportService>(MockBehavior.Strict);
        matchImport
            .Setup(x => x.ImportMatchesForSeasonAsync(
                season,
                LeagueExternalId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(DateTime.UtcNow);

        var cache = new Mock<IMemoryCache>(MockBehavior.Loose);
        var service = new LeagueService(
            leagueRepo.Object,
            seasonRepo.Object,
            teamImport.Object,
            matchImport.Object,
            sportsDb.Object,
            cache.Object);

        // Act
        bool result =
            await service.SetLeagueVisibilityAsync(LeagueId, true);

        // Assert
        result.Should().BeTrue();
        season.MatchesImported.Should().BeTrue();

        leagueRepo.VerifyAll();
        seasonRepo.VerifyAll();
        teamImport.VerifyAll();
        matchImport.VerifyAll();
        sportsDb.VerifyAll();
    }
}
