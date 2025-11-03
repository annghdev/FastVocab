using FastVocab.Application.Common.Interfaces;
using FastVocab.Shared.DTOs.Practice;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.PracticeSessions.Commands.CreatePracticeSession;

public record CreatePracticeSessionCommand(CreatePracticeSessionRequest Request) 
    : IRequest<Result<PracticeSessionDto>>, ICacheInvalidatorRequest
{
    public IEnumerable<string> CacheKeysToInvalidate => ["practices_all", $"practices_user_{Request.UserId}"];

    public string? Prefix => "practices:";
}
