public sealed record LoginResult(
    bool Success,
    string? Token,
    string? Error)
{
    public static LoginResult InvalidCredentials() =>
        new(false, null, "Invalid email or password");

    public static LoginResult Ok(string token) =>
        new(true, token, null);
}
