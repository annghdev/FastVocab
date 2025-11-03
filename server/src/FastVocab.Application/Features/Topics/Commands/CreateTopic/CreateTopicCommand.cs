using FastVocab.Application.Common.Interfaces;
using FastVocab.Shared.DTOs.Topics;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Topics.Commands.CreateTopic;

/// <summary>
/// Command to create a new Topic
/// </summary>
public record CreateTopicCommand(CreateTopicRequest Request) : IRequest<Result<TopicDto>>, ICacheInvalidatorRequest
{
    public IEnumerable<string> CacheKeysToInvalidate => ["topics_all", "topics_visible"];

    public string? Prefix => "topics:"!;
}

