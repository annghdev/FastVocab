using FastVocab.Application.Common.Interfaces;
using FastVocab.Shared.DTOs.Words;
using MediatR;

namespace FastVocab.Application.Features.Words.Queries.GetAllWords;

/// <summary>
/// Query to get all Words
/// </summary>
public record GetAllWordsQuery : IRequest<IEnumerable<WordDto>>, ICacheableRequest
{
    public string? CacheKey => "words_all";

    public TimeSpan? ExpirationSliding => TimeSpan.FromMinutes(15);
}

