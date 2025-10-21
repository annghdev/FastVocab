using FastVocab.Shared.DTOs.Collections;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Commands.UpdateCollection;

/// <summary>
/// Command to update an existing Collection
/// </summary>
public record UpdateCollectionCommand(UpdateCollectionRequest Request) : IRequest<Result<CollectionDto>>;
