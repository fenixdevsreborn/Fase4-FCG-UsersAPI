using System.Net.Http.Json;
using UsersAPI.Application.DTOs.Auth.Login;

namespace UsersAPI.IntegrationTests.Helpers;

public static class AuthHelper
{
    public static async Task<string> AuthenticateAsUserAsync(HttpClient client)
    {
        var request = new LoginRequest
        {
            Email = "test@usersapi.com",
            Password = "Test!123"
        };

        return await AuthenticateAsync(client, request);
    }

    public static async Task<string> AuthenticateAsAdminAsync(HttpClient client)
    {
        var request = new LoginRequest
        {
            Email = "testadm@usersapi.com",
            Password = "TstAdmin123!"
        };

        return await AuthenticateAsync(client, request);
    }

    private static async Task<string> AuthenticateAsync(
    HttpClient client,
    LoginRequest request)
    {
        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<LoginResponse>();

        return result!.AccessToken;
    }

    private sealed class LoginResponseDto
    {
        public string AccessToken { get; set; } = default!;
    }
}
