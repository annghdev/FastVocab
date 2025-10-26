using FastVocab.Application.Common.Interfaces;
using FastVocab.Shared.DTOs.Topics;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Topics.Queries.GetTopicById;

/// <summary>
/// Query to get a Topic by ID
/// </summary>
public record GetTopicByIdQuery(int TopicId) : IRequest<Result<TopicDto>>, ICacheableRequest
{
    public string? CacheKey => $"topic_{TopicId}";
    public TimeSpan? ExpirationSliding => TimeSpan.FromMinutes(10);
}