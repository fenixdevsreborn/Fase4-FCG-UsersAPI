public sealed class JwtOptions
{
    public string Key { get; init; } = default!;
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public int ExpiresInMinutes { get; init; }
}