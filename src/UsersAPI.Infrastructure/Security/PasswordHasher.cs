using System.Security.Cryptography;
using System.Text;
using UsersAPI.Application.Interfaces;
using UsersAPI.Domain.ValueObjects;

namespace UsersAPI.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(Password password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password.Value);
        var hash = sha256.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }

    public bool Verify(Password password, string passwordHash)
    {
        var computedHash = Hash(password);
        return computedHash == passwordHash;
    }
}