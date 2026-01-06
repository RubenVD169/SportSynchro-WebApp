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
using SportSynchro.Application.Models.Sports;

using SportSynchro.Domain.Entities;
using SportSynchro.Domain.ValueObjects;

namespace SportSynchro.Application.Tests.Services;

public sealed class SportServiceTests
{
    // Helpers
    private static Sport CreateSport(int id, string name, bool isVisible)
    {
        var sport = new Sport(
            externalId: 10,
            name: SportName.Create(name),
            isVisible: isVisible);

        SetPrivateProperty(sport, nameof(Sport.Id), id);
        return sport;
    }

    private static void SetPrivateProperty<T>(T instance, string propertyName, object value)
    {
        var prop = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        prop.Should().NotBeNull();

        var setter = prop!.GetSetMethod(nonPublic: true);
        setter.Should().NotBeNull();

        setter!.Invoke(instance, new[] { value });
    }

    // GetAllForAdminAsync
    [Fact]
    public async Task GetAllForAdminAsync_ReturnsMappedAdminModels()
    {
        // Arrange
        var sports = new List<Sport>
        {
            CreateSport(1, "Football", true),
            CreateSport(2, "Basketball", false)
        };

        var repo = new Mock<ISportRepository>(MockBehavior.Strict);
        repo
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sports);

        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new SportService(repo.Object, cache);

        // Act
        IReadOnlyList<SportAdminModel> result =
            await service.GetAllForAdminAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Football");
        result[0].IsVisible.Should().BeTrue();
        result[1].IsVisible.Should().BeFalse();

        repo.VerifyAll();
    }

    // GetAllForUserAsync
    [Fact]
    public async Task GetAllForUserAsync_ReturnsOnlyVisibleSports()
    {
        // Arrange
        var sports = new List<Sport>
        {
            CreateSport(1, "Football", true),
            CreateSport(2, "Basketball", true)
        };

        var repo = new Mock<ISportRepository>(MockBehavior.Strict);
        repo
            .Setup(x => x.GetAllVisibleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sports);

        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new SportService(repo.Object, cache);

        // Act
        IReadOnlyList<SportUserModel> result =
            await service.GetAllForUserAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Football");

        repo.VerifyAll();
    }

    // SetSportVisibilityAsync
    [Fact]
    public async Task SetSportVisibilityAsync_SportNotFound_ReturnsFalse()
    {
        // Arrange
        var repo = new Mock<ISportRepository>(MockBehavior.Strict);
        repo
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sport?)null);

        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new SportService(repo.Object, cache);

        // Act
        bool result =
            await service.SetSportVisibilityAsync(1, true);

        // Assert
        result.Should().BeFalse();
        repo.VerifyAll();
    }

    [Fact]
    public async Task SetSportVisibilityAsync_SaveReturnsZero_ReturnsFalse()
    {
        // Arrange
        var sport = CreateSport(1, "Football", false);

        var repo = new Mock<ISportRepository>(MockBehavior.Strict);
        repo
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sport);

        repo
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new SportService(repo.Object, cache);

        // Act
        bool result =
            await service.SetSportVisibilityAsync(1, true);

        // Assert
        result.Should().BeFalse();
        sport.IsVisible.Should().BeTrue();

        repo.VerifyAll();
    }

    [Fact]
    public async Task SetSportVisibilityAsync_SaveSucceeds_ReturnsTrue()
    {
        // Arrange
        var sport = CreateSport(1, "Football", false);

        var repo = new Mock<ISportRepository>(MockBehavior.Strict);
        repo
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sport);

        repo
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new SportService(repo.Object, cache);

        // Act
        bool result =
            await service.SetSportVisibilityAsync(1, true);

        // Assert
        result.Should().BeTrue();
        sport.IsVisible.Should().BeTrue();

        repo.VerifyAll();
    }
}
