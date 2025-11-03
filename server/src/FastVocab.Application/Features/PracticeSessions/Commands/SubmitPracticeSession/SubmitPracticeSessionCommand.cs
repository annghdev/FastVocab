using FastVocab.Application.Common.Interfaces;
using FastVocab.Shared.DTOs.Practice;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.PracticeSessions.Commands.SubmitPracticeSession;

public record SubmitPracticeSessionCommand(int Id, Guid UserId) 
    : IRequest<Result<PracticeSessionDto>>, ICacheInvalidatorRequest
{
    public IEnumerable<string> CacheKeysToInvalidate => ["practices_all", $"practices_user_{UserId}" ,$"practice_{Id}"];

    public string? Prefix => "practices:";
}
