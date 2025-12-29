using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Models.Sports;
using SportSynchro.Api.Contracts.Sports.Responses;

namespace SportSynchro.Api.Tests;

public sealed class UserSportsApiTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UserSportsApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
{
    builder.ConfigureServices(services =>
    {
        // Replace authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = FakeAuthHandler.SchemeName;
            options.DefaultChallengeScheme = FakeAuthHandler.SchemeName;
        })
        .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>(
            FakeAuthHandler.SchemeName, _ => { });

        // Remove real ISportService
        var descriptor = services.Single(
            d => d.ServiceType == typeof(ISportService));
        services.Remove(descriptor);

        // Mock ISportService
        var sportServiceMock = new Mock<ISportService>();
        sportServiceMock
            .Setup(x => x.GetAllForUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SportUserModel>
            {
                new(1, "Football"),
                new(2, "Basketball")
            });

        services.AddSingleton(sportServiceMock.Object);
    });
});

    }

    [Fact]
    public async Task GetAllForUser_ReturnsOkWithMappedSports()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        HttpResponseMessage response =
            await client.GetAsync("/api/user/sports");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        IReadOnlyList<SportUserResponse>? content =
            await response.Content
                .ReadFromJsonAsync<IReadOnlyList<SportUserResponse>>();

        content.Should().NotBeNull();
        content.Should().HaveCount(2);
        content![0].Name.Should().Be("Football");
        content![1].Name.Should().Be("Basketball");
    }
}
