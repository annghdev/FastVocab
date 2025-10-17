using FastVocab.Domain.Entities.Abstractions;

namespace FastVocab.Domain.Entities;

public class AppUser : AuditableEntityBase<Guid>
{
    public string? FullName { get; set; } = "Guest";
    public string? SessionId { get; set; }
    public string? AccountId { get; set; }
}
