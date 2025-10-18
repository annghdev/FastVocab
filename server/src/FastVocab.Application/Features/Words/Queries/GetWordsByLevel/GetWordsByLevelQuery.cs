using FastVocab.Shared.DTOs.Words;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Queries.GetWordsByLevel;

/// <summary>
/// Query to get Words by difficulty Level
/// </summary>
public record GetWordsByLevelQuery(string Level) : IRequest<Result<IEnumerable<WordDto>>>;

