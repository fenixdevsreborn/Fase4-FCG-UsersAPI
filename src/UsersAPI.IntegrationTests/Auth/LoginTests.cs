using FluentAssertions;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using UsersAPI.Application.DTOs.Auth.Login;
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

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrWhiteSpace();

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub);
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "test@usersapi.com");
        jwt.Claims.Should().Contain(c => c.Type == "name" && c.Value == "Test User");
        jwt.Claims.Should().Contain(c => c.Type == "role" && c.Value == "User");
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
