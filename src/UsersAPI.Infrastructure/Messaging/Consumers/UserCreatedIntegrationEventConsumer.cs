using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;

namespace UsersAPI.Infrastructure.Messaging.Consumers
{
    public class UserCreatedIntegrationEventConsumer
        : IConsumer<UserCreatedIntegrationEvent>
    {
        private readonly ILogger<UserCreatedIntegrationEventConsumer> _logger;

        public UserCreatedIntegrationEventConsumer(ILogger<UserCreatedIntegrationEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<UserCreatedIntegrationEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                $"[RabbitMQ] User created: {message.UserId} | {message.Email} | {message.OccurredAt}");

            return Task.CompletedTask;
        }
    }
}
