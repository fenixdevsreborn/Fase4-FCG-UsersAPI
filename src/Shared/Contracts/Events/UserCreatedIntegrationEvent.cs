namespace Shared.Contracts.Events;

public class UserCreatedIntegrationEvent
{
    public Guid UserId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public DateTime OccurredAt { get; private set; }

    protected UserCreatedIntegrationEvent() { }

    public UserCreatedIntegrationEvent(Guid userId, string email, DateTime occurredAt)
    {
        UserId = userId;
        Email = email;
        OccurredAt = occurredAt;
    }
}