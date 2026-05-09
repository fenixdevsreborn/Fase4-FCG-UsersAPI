using Microsoft.Extensions.DependencyInjection;
using UsersAPI.Application.Abstractions;
using UsersAPI.Application.Events;

namespace UsersAPI.Infrastructure.Messaging;

public sealed class DomainEventDispatcher
    : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync(IEnumerable<object> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            var handlerType = typeof(IEventHandler<>)
                .MakeGenericType(domainEvent.GetType());

            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod("HandleAsync");
                if (method is null)
                    continue;

                await (Task)method.Invoke(handler, new[] { domainEvent, null })!;
            }
        }
    }
}
