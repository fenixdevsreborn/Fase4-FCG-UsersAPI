using UsersAPI.Domain.Events;

namespace UsersAPI.Application.Events;

public interface IEventHandler<in TEvent>
{
    Task HandleAsync(UserCreatedEvent domainEvent, CancellationToken cancellationToken = default);
}