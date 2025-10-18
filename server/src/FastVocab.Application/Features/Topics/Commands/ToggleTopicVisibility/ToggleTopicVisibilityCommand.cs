using FastVocab.Shared.DTOs.Topics;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Topics.Commands.ToggleTopicVisibility;

/// <summary>
/// Command to toggle Topic visibility (IsHiding)
/// </summary>
public record ToggleTopicVisibilityCommand(int TopicId) : IRequest<Result<TopicDto>>;

