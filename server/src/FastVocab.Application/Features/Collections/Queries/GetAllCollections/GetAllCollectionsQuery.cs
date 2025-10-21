using FastVocab.Shared.DTOs.Collections;
using MediatR;

namespace FastVocab.Application.Features.Collections.Queries.GetAllCollections;

/// <summary>
/// Query to get all Collections (including hidden ones for admin)
/// </summary>
public record GetAllCollectionsQuery : IRequest<IEnumerable<CollectionDto>>;
