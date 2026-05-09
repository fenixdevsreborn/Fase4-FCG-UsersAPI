namespace UsersAPI.Domain.ValueObjects;

/// <summary>
/// Represents a valid plain password before hashing.
/// </summary>
public sealed class Password
{
    public string Value { get; }

    public Password(string value, bool isLogin = false)
    {
        if (!isLogin) { 
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Password cannot be empty.", nameof(value));

            if (value.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters long.", nameof(value));

            if (!value.Any(char.IsUpper))
                throw new ArgumentException("Password must contain at least one uppercase letter.", nameof(value));

            if (!value.Any(char.IsLower))
                throw new ArgumentException("Password must contain at least one lowercase letter.", nameof(value));

            if (!value.Any(char.IsDigit))
                throw new ArgumentException("Password must contain at least one digit.", nameof(value));
        }
        Value = value;
    }

    public override string ToString()
        => "********"; // never expose the real value
}
