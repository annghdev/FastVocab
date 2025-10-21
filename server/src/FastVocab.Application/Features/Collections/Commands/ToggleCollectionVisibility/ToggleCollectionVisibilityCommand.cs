using FastVocab.Shared.DTOs.Collections;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Commands.ToggleCollectionVisibility;

/// <summary>
/// Command to toggle Collection visibility
/// </summary>
public record ToggleCollectionVisibilityCommand(int Id) : IRequest<Result<CollectionDto>>;
