using FastVocab.Domain.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastVocab.Domain.Entities.Abstractions;

public abstract class EntityBase<TKey> : IEntity
{
    public TKey Id { get; set; } = default!;

    [NotMapped]
    public List<DomainEventBase> DomainEvents { get; set; } = [];

    public void AddDomainEvent(DomainEventBase evt)
    {
        DomainEvents.Add(evt);
    }

    public void ClearDomainEvents()
    {
        DomainEvents.Clear();
    }
}
