using FastVocab.Shared.DTOs.Topics;
using MediatR;

namespace FastVocab.Application.Features.Topics.Queries.GetAllTopics;

/// <summary>
/// Query to get all Topics (including hidden ones for admin)
/// </summary>
public record GetAllTopicsQuery : IRequest<IEnumerable<TopicDto>>;

