namespace FixDesk.Domain.Abstractions.Events;

public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    protected DomainEvent() { }
}
