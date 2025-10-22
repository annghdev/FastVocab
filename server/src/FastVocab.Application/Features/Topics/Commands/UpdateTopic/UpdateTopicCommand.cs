using FastVocab.Application.Common.Interfaces;
using FastVocab.Shared.DTOs.Topics;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Topics.Commands.UpdateTopic;

/// <summary>
/// Command to update an existing Topic
/// </summary>
public record UpdateTopicCommand(UpdateTopicRequest Request) : IRequest<Result<TopicDto>>, ICacheInvalidatorRequest
{
    public IEnumerable<string> CacheKeysToInvalidate => ["AllTopics", "VisibleTopics", $"Topic_{Request.Id}"];

    public IEnumerable<string>? CacheKeysPattern => null;
}