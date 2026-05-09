using MediatR;

namespace UsersAPI.Domain.Events
{
    public sealed record UserCreatedEvent(
        Guid UserId,
        string Email,
        DateTime CreatedAt
    ) : INotification;
}
