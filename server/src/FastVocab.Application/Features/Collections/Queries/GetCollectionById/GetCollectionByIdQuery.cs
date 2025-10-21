using FastVocab.Shared.DTOs.Collections;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Queries.GetCollectionById;

/// <summary>
/// Query to get Collection by ID
/// </summary>
public record GetCollectionByIdQuery(int Id) : IRequest<Result<CollectionDto>>;
