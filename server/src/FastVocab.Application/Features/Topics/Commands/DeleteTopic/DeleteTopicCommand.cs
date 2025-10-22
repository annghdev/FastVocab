using FastVocab.Application.Common.Interfaces;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Topics.Commands.DeleteTopic;

/// <summary>
/// Command to soft delete a Topic
/// </summary>
public record DeleteTopicCommand(int TopicId) : IRequest<Result>, ICacheInvalidatorRequest
{
    public IEnumerable<string> CacheKeysToInvalidate => ["AllTopics", "VisibleTopics", $"Topic_{TopicId}"];

    public IEnumerable<string>? CacheKeysPattern => null;
}

