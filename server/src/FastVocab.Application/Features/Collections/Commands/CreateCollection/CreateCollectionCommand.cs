using FastVocab.Shared.DTOs.Collections;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Commands.CreateCollection;

/// <summary>
/// Command to create a new Collection
/// </summary>
public record CreateCollectionCommand(CreateCollectionRequest Request) : IRequest<Result<CollectionDto>>;
