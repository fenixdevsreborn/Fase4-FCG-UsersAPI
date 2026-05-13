using FluentAssertions;
using UsersAPI.Domain.Entities;
using UsersAPI.Domain.Events;
using UsersAPI.Domain.ValueObjects;
using static UsersAPI.Domain.Entities.User;

namespace UsersAPI.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Create_should_raise_UserCreatedEvent()
    {
        // Arrange
        var name = "John Doe";
        var email = new Email("john@doe.com");
        var passwordHash = "hashed-password";

        // Act
        var user = new User(
            name,
            email,
            passwordHash,
            UserRole.User
        );

        // Assert

        var domainEvent = Assert.Single(user.DomainEvents);
        var createdEvent = Assert.IsType<UserCreatedEvent>(domainEvent);

        createdEvent.UserId.Should().Be(user.Id);
        createdEvent.Email.Should().Be(email.Value);
        createdEvent.CreatedAt.Should().Be(user.CreatedAt);
    }

    [Fact]
    public void PromoteToAdmin_should_change_user_role_to_admin()
    {
        var user = new User(
            "John Doe",
            new Email("john@doe.com"),
            "hashed-password",
            UserRole.User
        );

        user.PromoteToAdmin();

        user.Role.Should().Be(UserRole.Admin);
        user.UpdatedAt.Should().NotBeNull();
    }
}
