
namespace FastVocab.Domain.Entities.Abstractions;

public class AuditableEntityBase<Tkey> : EntityBase<Task>, IAuditable, ISoftDeletable
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}
