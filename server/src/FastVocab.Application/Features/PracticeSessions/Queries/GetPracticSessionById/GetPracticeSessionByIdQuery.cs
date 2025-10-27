using FastVocab.Application.Common.Interfaces;
using FastVocab.Shared.DTOs.Practice;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.PracticeSessions.Queries.GetPracticSessionById;

public record GetPracticeSessionByIdQuery(int Id) : IRequest<Result<PracticeSessionDto>>, ICacheableRequest
{
    public string? CacheKey => $"practice_{Id}";

    public TimeSpan? ExpirationSliding => TimeSpan.FromMinutes(15);
}
