using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Topics.Commands.DeleteTopic;

/// <summary>
/// Command to soft delete a Topic
/// </summary>
public record DeleteTopicCommand(int TopicId) : IRequest<Result>;

