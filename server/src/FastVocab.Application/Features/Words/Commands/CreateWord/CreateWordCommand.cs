using FastVocab.Application.Common.Interfaces;
using FastVocab.Shared.DTOs.Words;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Commands.CreateWord;

/// <summary>
/// Command to create a new Word with optional topics
/// </summary>
public record CreateWordCommand(CreateWordRequest Request) : IRequest<Result<WordDto>>, ICacheInvalidatorRequest
{
    public IEnumerable<string> CacheKeysToInvalidate => ["words_all", .. Request.TopicIds?.Select(t => $"words_topic_{t}").ToList()];

    public string? Prefix => "words:";
}

