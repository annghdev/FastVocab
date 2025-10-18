namespace FastVocab.Domain.Entities.Abstractions;

public abstract class AuditableEntityBase<Tkey> : EntityBase<Tkey>, IAuditable, ISoftDeletable
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}
