using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastVocab.Domain.Entities.Abstractions;

public abstract class EntityBase<TKey> : IEntity
{
    [Key]
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
