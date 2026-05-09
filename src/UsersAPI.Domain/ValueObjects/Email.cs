using System.Text.RegularExpressions;

namespace UsersAPI.Domain.ValueObjects;

/// <summary>
/// Represents an immutable email value object.
/// </summary>
public sealed class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.", nameof(value));

        var normalized = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(normalized))
            throw new ArgumentException("Invalid email format.", nameof(value));

        Value = normalized;
    }

    public override string ToString() => Value;

    #region Equality

    public bool Equals(Email? other)
        => other is not null && Value == other.Value;

    public override bool Equals(object? obj)
        => obj is Email other && Equals(other);

    public override int GetHashCode()
        => Value.GetHashCode(StringComparison.Ordinal);

    public static bool operator ==(Email left, Email right)
        => Equals(left, right);

    public static bool operator !=(Email left, Email right)
        => !Equals(left, right);

    #endregion
}
