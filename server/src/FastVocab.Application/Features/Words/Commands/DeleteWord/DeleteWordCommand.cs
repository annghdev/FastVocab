using FastVocab.Application.Common.Interfaces;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Commands.DeleteWord;

/// <summary>
/// Command to soft delete a Word
/// </summary>
public record DeleteWordCommand(int WordId) : IRequest<Result>, ICacheInvalidatorRequest
{
    public IEnumerable<string> CacheKeysToInvalidate => ["words_all"];

    public string? Prefix => "words:";
}

