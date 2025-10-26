using FastVocab.Application.Common.Interfaces;
using FastVocab.Shared.DTOs.Topics;
using MediatR;

namespace FastVocab.Application.Features.Topics.Queries.GetAllTopics;

/// <summary>
/// Query to get all Topics (including hidden ones for admin)
/// </summary>
public record GetAllTopicsQuery : IRequest<IEnumerable<TopicDto>>, ICacheableRequest
{
    public string? CacheKey => "topics_all";
    public TimeSpan? ExpirationSliding => TimeSpan.FromMinutes(15);
}

