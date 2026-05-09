namespace UsersAPI.Application.DTOs.Auth.Login
{
    public sealed class LoginRequest
    {
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
    }
}
