using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;
using UsersAPI.Domain.Events;

namespace UsersAPI.Application.Events;

public sealed class UserCreatedEventHandler
    : IEventHandler<UserCreatedEvent>
{
    private readonly ILogger<UserCreatedEventHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;


    public UserCreatedEventHandler(
        ILogger<UserCreatedEventHandler> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task HandleAsync(UserCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "User created | Id: {UserId} | Email: {Email} | At: {CreatedAt}",
            domainEvent.UserId,
            domainEvent.Email,
            domainEvent.CreatedAt
        );

        _logger.LogInformation(
            "Publishing UserCreatedIntegrationEvent for UserId {UserId}",
            domainEvent.UserId
        );

        var integrationEvent = new UserCreatedIntegrationEvent(
            domainEvent.UserId,
            domainEvent.Email,
            DateTime.UtcNow
        );

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);
    }
}
