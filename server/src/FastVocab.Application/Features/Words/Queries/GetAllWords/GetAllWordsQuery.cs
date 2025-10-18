using FastVocab.Shared.DTOs.Words;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Queries.GetAllWords;

/// <summary>
/// Query to get all Words
/// </summary>
public record GetAllWordsQuery : IRequest<IEnumerable<WordDto>>;

