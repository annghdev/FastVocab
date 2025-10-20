namespace FastVocab.Shared.DTOs.Users.Requests;

public record UpdateUserRequest
{
    public Guid Id { get; init; }
    public string? FullName { get; init; }
    public string? SessionId { get; init; }
    public Guid? AccountId { get; init; }
}
