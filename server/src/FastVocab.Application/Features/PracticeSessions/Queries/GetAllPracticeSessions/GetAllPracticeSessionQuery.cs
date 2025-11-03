using FastVocab.Application.Common.Interfaces;
using FastVocab.Shared.DTOs.Practice;
using MediatR;

namespace FastVocab.Application.Features.PracticeSessions.Queries.GetAllPracticeSessions;

public record GetAllPracticeSessionQuery : IRequest<IEnumerable<PracticeSessionDto>>, ICacheableRequest
{
    public string? CacheKey => "practices_all";

    public TimeSpan? ExpirationSliding => TimeSpan.FromMinutes(10);
}
