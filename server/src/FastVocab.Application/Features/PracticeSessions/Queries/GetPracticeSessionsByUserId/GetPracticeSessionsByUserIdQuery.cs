using FastVocab.Application.Common.Interfaces;
using FastVocab.Shared.DTOs.Practice;
using MediatR;

namespace FastVocab.Application.Features.PracticeSessions.Queries.GetPracticeSessionsByUserId;

public record GetPracticeSessionsByUserIdQuery(Guid UserId) : IRequest<IEnumerable<PracticeSessionDto>>, ICacheableRequest
{
    public string? CacheKey => $"practices_user_{UserId}";

    public TimeSpan? ExpirationSliding => TimeSpan.FromMinutes(15);
}