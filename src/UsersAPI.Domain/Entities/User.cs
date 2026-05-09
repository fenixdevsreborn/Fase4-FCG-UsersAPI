using UsersAPI.Domain.Events;
using UsersAPI.Domain.ValueObjects;

namespace UsersAPI.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }

    protected User() { }

    public User(string name, Email email, string passwordHash, UserRole role = 0)
        : base()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        Email = email ?? throw new ArgumentNullException(nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

        Name = name;
        PasswordHash = passwordHash;
        Role = role;

        this.AddDomainEvent(
            new UserCreatedEvent(
                this.Id,
                this.Email.Value,
                this.CreatedAt
            )
        );
    }

    public void ChangeName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name cannot be empty.", nameof(newName));

        Name = newName;
        Touch();
    }

    public void ChangeEmail(Email newEmail)
    {
        Email = newEmail ?? throw new ArgumentNullException(nameof(newEmail));
        Touch();
    }

    public void ChangePasswordHash(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
        Touch();
    }

    public enum UserRole
    {
        User = 0,
        Admin = 1
    }
}
