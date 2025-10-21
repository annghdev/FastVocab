using FastVocab.Shared.DTOs.Collections;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Queries.GetCollectionWithLists;

/// <summary>
/// Query to get Collection with its WordLists
/// </summary>
public record GetCollectionWithListsQuery(int Id) : IRequest<Result<CollectionDto>>; // Assume CollectionDto has WordLists
