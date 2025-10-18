using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Commands.AddWordToTopic;

/// <summary>
/// Command to add a Word to a Topic
/// </summary>
public record AddWordToTopicCommand(int WordId, int TopicId) : IRequest<Result>;

