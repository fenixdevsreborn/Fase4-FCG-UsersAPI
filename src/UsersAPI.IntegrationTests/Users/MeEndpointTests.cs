using System.Net;
using FluentAssertions;
using UsersAPI.IntegrationTests.Fixtures;
using UsersAPI.IntegrationTests.Helpers;

namespace UsersAPI.IntegrationTests.Users;

public sealed class MeEndpointTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MeEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_access_protected_endpoint_with_valid_token()
    {
        var token = await AuthHelper.AuthenticateAsUserAsync(_client);

        _client.DefaultRequestHeaders.Authorization =
            new("Bearer", token);

        var response = await _client.GetAsync("/api/users/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_return_401_without_token()
    {
        var response = await _client.GetAsync("/api/users/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
