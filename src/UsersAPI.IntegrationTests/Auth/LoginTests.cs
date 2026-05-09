using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using UsersAPI.IntegrationTests.Fixtures;

namespace UsersAPI.IntegrationTests.Auth;

public sealed class LoginTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LoginTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_login_successfully()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = "test@usersapi.com",
            Password = "Test!123"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_return_401_when_password_is_invalid()
    {
        var request = new
        {
            Email = "test@usersapi.com",
            Password = "wrong-password"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

}
