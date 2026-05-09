using Microsoft.Extensions.DependencyInjection;
using UsersAPI.Application.DTOs.Auth.Login;
using UsersAPI.Application.Events;
using UsersAPI.Domain.Events;

namespace UsersAPI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<LoginHandler>();
        services.AddScoped<
            IEventHandler<UserCreatedEvent>,
            UserCreatedEventHandler>();

        return services;
    }
}
