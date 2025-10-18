using FastVocab.Shared.DTOs.Topics;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Topics.Queries.GetVisibleTopics;

/// <summary>
/// Query to get only visible Topics (IsHiding = false) for students
/// </summary>
public record GetVisibleTopicsQuery : IRequest<Result<IEnumerable<TopicDto>>>;

