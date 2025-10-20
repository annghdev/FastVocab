namespace FastVocab.Shared.DTOs.Users.Requests;

public record CreateUserRequest
{
    public string? FullName { get; init; }
    public string? SessionId { get; init; }
    public Guid? AccountId { get; init; }
}
