namespace FastVocab.Shared.DTOs.Users;

/// <summary>
/// DTO for displaying User information
/// </summary>
public record UserDto
{
    public Guid Id { get; init; }
    public string? FullName { get; init; }
    public Guid? AccountId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

