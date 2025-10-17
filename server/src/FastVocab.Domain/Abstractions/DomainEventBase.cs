using MediatR;

namespace FastVocab.Domain.Abstractions;

public abstract record DomainEventBase : INotification
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public DateTimeOffset OccuredAt { get; init; } = DateTimeOffset.UtcNow;
}
