using FastVocab.Shared.DTOs.Words;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Queries.GetWordsByTopic;

/// <summary>
/// Query to get Words by Topic ID
/// </summary>
public record GetWordsByTopicQuery(int TopicId) : IRequest<Result<IEnumerable<WordDto>>>;

