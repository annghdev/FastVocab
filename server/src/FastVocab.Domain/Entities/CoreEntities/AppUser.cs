using FastVocab.Domain.Entities.Abstractions;

namespace FastVocab.Domain.Entities.CoreEntities;

public class AppUser : AuditableEntityBase<Guid>
{
    public string? FullName { get; set; } = "Guest";
    public string? SessionId { get; set; }
    public Guid? AccountId { get; set; }
}
