using FastVocab.Application.Common.Interfaces;
using FastVocab.Shared.DTOs.Words;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Queries.GetWordById;

/// <summary>
/// Query to get a Word by ID
/// </summary>
public record GetWordByIdQuery(int WordId) : IRequest<Result<WordDto>>, ICacheableRequest
{
    public string? CacheKey => $"word_{WordId}";

    public TimeSpan? ExpirationSliding => TimeSpan.FromMinutes(15);
}

