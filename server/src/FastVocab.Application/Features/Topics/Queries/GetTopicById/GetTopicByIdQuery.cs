using FastVocab.Shared.DTOs.Topics;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Topics.Queries.GetTopicById;

/// <summary>
/// Query to get a Topic by ID
/// </summary>
public record GetTopicByIdQuery(int TopicId) : IRequest<Result<TopicDto>>;

