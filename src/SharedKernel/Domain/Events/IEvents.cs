namespace SharedKernel.Domain.Events;
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}
public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}
