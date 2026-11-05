namespace FixDesk.Domain.Abstractions.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}