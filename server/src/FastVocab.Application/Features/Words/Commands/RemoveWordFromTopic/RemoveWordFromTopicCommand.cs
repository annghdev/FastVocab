using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Commands.RemoveWordFromTopic;

/// <summary>
/// Command to remove a Word from a Topic
/// </summary>
public record RemoveWordFromTopicCommand(int WordId, int TopicId) : IRequest<Result>;

