using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

using SportSynchro.External.TheSportsDb.Contracts.Models.Teams;


namespace SportSynchro.Application.Tests.Services;

public sealed class TeamImportServiceTests
{
    private const int LeagueExternalId = 99;

    private static Season CreateSeasonWithId(int seasonId)
    {
        var season = new Season(
            leagueId: 10,
            key: SeasonKey.Create("2023-2024"),
            isCurrent: true);

        SetPrivateProperty(season, nameof(Season.Id), seasonId);
        return season;
    }

    private static Team CreateTeamWithId(int id, int externalId, string name = "Team A")
    {
        var team = new Team(
            externalId,
            TeamName.Create(name),
            "Belgium");

        SetPrivateProperty(team, nameof(Team.Id), id);
        return team;
    }

    private static TheSportsDbTeamDto CreateDto(
        string idTeam,
        string name,
        string? country = "Belgium")
        => new()
        {
            IdTeam = idTeam,
            StrTeam = name,
            StrCountry = country
        };

    private static void SetPrivateProperty<T>(T instance, string propertyName, object value)
    {
        var prop = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        prop.Should().NotBeNull();

        var setter = prop!.GetSetMethod(nonPublic: true);
        setter.Should().NotBeNull();

        setter!.Invoke(instance, new[] { value });
    }

    // API returns no teams -> early return
    [Fact]
    public async Task ImportTeamsForSeasonAsync_NoApiTeams_ReturnsEarly()
    {
        // Arrange
        var season = CreateSeasonWithId(1);

        var sportsDb = new Mock<ITheSportsDbRepository>(MockBehavior.Strict);
        sportsDb
            .Setup(x => x.GetTeamsByLeagueAsync(
                LeagueExternalId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TheSportsDbTeamDto>());

        var teamRepo = new Mock<ITeamRepository>(MockBehavior.Strict);
        var seasonTeamRepo = new Mock<ISeasonTeamRepository>(MockBehavior.Strict);

        var service = new TeamImportService(
            sportsDb.Object,
            teamRepo.Object,
            seasonTeamRepo.Object);

        // Act
        await service.ImportTeamsForSeasonAsync(season, LeagueExternalId);

        // Assert
        sportsDb.VerifyAll();
        teamRepo.VerifyNoOtherCalls();
        seasonTeamRepo.VerifyNoOtherCalls();
    }

    // All teams already exist + already linked -> no inserts
    [Fact]
    public async Task ImportTeamsForSeasonAsync_AllTeamsExist_NoNewTeamsOrLinks()
    {
        // Arrange
        var season = CreateSeasonWithId(1);

        var dto = CreateDto("100", "Team A");
        var existingTeam = CreateTeamWithId(10, 100);

        var sportsDb = new Mock<ITheSportsDbRepository>(MockBehavior.Strict);
        sportsDb
            .Setup(x => x.GetTeamsByLeagueAsync(
                LeagueExternalId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { dto });

        var teamRepo = new Mock<ITeamRepository>(MockBehavior.Strict);
        teamRepo
            .Setup(x => x.GetByExternalIdsAsync(
                It.IsAny<HashSet<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, Team>
            {
                { 100, existingTeam }
            });

        var seasonTeamRepo = new Mock<ISeasonTeamRepository>(MockBehavior.Strict);
        seasonTeamRepo
            .Setup(x => x.GetTeamIdsForSeasonAsync(
                season.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<int> { existingTeam.Id });

        var service = new TeamImportService(
            sportsDb.Object,
            teamRepo.Object,
            seasonTeamRepo.Object);

        // Act
        await service.ImportTeamsForSeasonAsync(season, LeagueExternalId);

        // Assert
        teamRepo.Verify(x => x.AddRangeAsync(It.IsAny<List<Team>>(), It.IsAny<CancellationToken>()), Times.Never);
        seasonTeamRepo.Verify(x => x.AddAsync(It.IsAny<SeasonTeam>(), It.IsAny<CancellationToken>()), Times.Never);

        sportsDb.VerifyAll();
        teamRepo.VerifyAll();
        seasonTeamRepo.VerifyAll();
    }

    // New teams detected -> teams saved + season links added
    [Fact]
    public async Task ImportTeamsForSeasonAsync_NewTeams_AddsTeamsAndSeasonTeams()
    {
        // Arrange
        var season = CreateSeasonWithId(1);

        var dto = CreateDto("200", "New Team");

        var sportsDb = new Mock<ITheSportsDbRepository>(MockBehavior.Strict);
        sportsDb
            .Setup(x => x.GetTeamsByLeagueAsync(
                LeagueExternalId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { dto });

        var teamRepo = new Mock<ITeamRepository>(MockBehavior.Strict);
        teamRepo
            .Setup(x => x.GetByExternalIdsAsync(
                It.IsAny<HashSet<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, Team>());

        teamRepo
            .Setup(x => x.AddRangeAsync(
                It.Is<List<Team>>(l => l.Count == 1),
                It.IsAny<CancellationToken>()))
            .Callback<IReadOnlyList<Team>, CancellationToken>((teams, _) =>
            {
                // Simulate EF Core identity assignment
                SetPrivateProperty(teams[0], nameof(Team.Id), 42);
            })
            .Returns(Task.CompletedTask);


        teamRepo
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var seasonTeamRepo = new Mock<ISeasonTeamRepository>(MockBehavior.Strict);
        seasonTeamRepo
            .Setup(x => x.GetTeamIdsForSeasonAsync(
                season.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<int>());

        seasonTeamRepo
            .Setup(x => x.AddAsync(It.IsAny<SeasonTeam>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        seasonTeamRepo
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new TeamImportService(
            sportsDb.Object,
            teamRepo.Object,
            seasonTeamRepo.Object);

        // Act
        await service.ImportTeamsForSeasonAsync(season, LeagueExternalId);

        // Assert
        teamRepo.Verify(x => x.AddRangeAsync(It.IsAny<List<Team>>(), It.IsAny<CancellationToken>()), Times.Once);
        teamRepo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        seasonTeamRepo.Verify(x => x.AddAsync(It.IsAny<SeasonTeam>(), It.IsAny<CancellationToken>()), Times.Once);
        seasonTeamRepo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        sportsDb.VerifyAll();
        teamRepo.VerifyAll();
        seasonTeamRepo.VerifyAll();
    }

    // Invalid DTOs skipped (invalid id or empty name)
    [Fact]
    public async Task ImportTeamsForSeasonAsync_InvalidDtos_AreSkipped()
    {
        // Arrange
        var season = CreateSeasonWithId(1);

        var invalidId = CreateDto("abc", "Team X");
        var invalidName = CreateDto("300", "");

        var sportsDb = new Mock<ITheSportsDbRepository>(MockBehavior.Strict);
        sportsDb
            .Setup(x => x.GetTeamsByLeagueAsync(
                LeagueExternalId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { invalidId, invalidName });

        var teamRepo = new Mock<ITeamRepository>(MockBehavior.Strict);
        teamRepo
            .Setup(x => x.GetByExternalIdsAsync(
                It.IsAny<HashSet<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, Team>());

        var seasonTeamRepo = new Mock<ISeasonTeamRepository>(MockBehavior.Strict);
        seasonTeamRepo
            .Setup(x => x.GetTeamIdsForSeasonAsync(
                season.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<int>());

        var service = new TeamImportService(
            sportsDb.Object,
            teamRepo.Object,
            seasonTeamRepo.Object);

        // Act
        await service.ImportTeamsForSeasonAsync(season, LeagueExternalId);

        // Assert
        teamRepo.Verify(x => x.AddRangeAsync(It.IsAny<List<Team>>(), It.IsAny<CancellationToken>()), Times.Never);
        seasonTeamRepo.Verify(x => x.AddAsync(It.IsAny<SeasonTeam>(), It.IsAny<CancellationToken>()), Times.Never);

        sportsDb.VerifyAll();
        teamRepo.VerifyAll();
        seasonTeamRepo.VerifyAll();
    }
}
